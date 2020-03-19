using System;
using System.Collections.Generic;
using System.Linq;
using Innoactive.Creator.Core;
using Innoactive.CreatorEditor.ImguiTester;
using Innoactive.CreatorEditor.UI.Graphics;
using Innoactive.CreatorEditor.UndoRedo;
using Innoactive.CreatorEditor.Utils;
using UnityEngine;

namespace Innoactive.CreatorEditor.UI.Windows
{
    public class ChapterRepresentation
    {
        public EditorGraphics Graphics { get; private set; }

        private IChapter currentChapter;
        private StepNode lastSelectedStepNode;

        private bool isUpdated = false;

        public Rect BoundingBox
        {
            get { return Graphics.BoundingBox; }
        }

        public ChapterRepresentation()
        {
            Graphics = new EditorGraphics(WorkflowEditorColorPalette.GetDefaultPalette());
        }

        private void SetupNode(EditorNode node, Action<Vector2> setPositionInModel)
        {
            Vector2 positionBeforeDrag = node.Position;

            node.GraphicalEventHandler.PointerDown += (sender, args) =>
            {
                positionBeforeDrag = node.Position;
            };

            node.GraphicalEventHandler.PointerUp += (sender, args) =>
            {
                if (Mathf.Abs((positionBeforeDrag - node.Position).sqrMagnitude) < 0.001f)
                {
                    return;
                }

                Vector2 positionAfterDrag = node.Position;
                Vector2 closuredPositionBeforeDrag = positionBeforeDrag;

                RevertableChangesHandler.Do(new TrainingCommand(() =>
                {
                    setPositionInModel(positionAfterDrag);
                    MarkToRefresh();
                }, () =>
                {
                    setPositionInModel(closuredPositionBeforeDrag);
                    MarkToRefresh();
                }));
            };

            node.GraphicalEventHandler.PointerDrag += (sender, args) =>
            {
                node.RelativePosition += args.PointerDelta;
            };
        }

        private void DeleteStepWithUndo(IStep step, StepNode ownerNode)
        {
            IList<ITransition> incomingTransitions = currentChapter.Data.Steps.SelectMany(s => s.Data.Transitions.Data.Transitions).Where(transition => transition.Data.TargetStep == step).ToList();

            bool wasFirstStep = step == currentChapter.Data.FirstStep;

            RevertableChangesHandler.Do(new TrainingCommand(
                () =>
                {
                    foreach (ITransition transition in incomingTransitions)
                    {
                        transition.Data.TargetStep = null;
                    }

                    DeleteStep(step);

                    if (wasFirstStep)
                    {
                        currentChapter.Data.FirstStep = null;
                    }
                },
                () =>
                {
                    AddStep(step);

                    if (wasFirstStep)
                    {
                        currentChapter.Data.FirstStep = step;
                    }

                    foreach (ITransition transition in incomingTransitions)
                    {
                        transition.Data.TargetStep = step;
                    }

                    SelectStepNode(ownerNode);
                }
            ));
        }

        private StepNode CreateNewStepNode(IStep step)
        {
            StepNode node = new StepNode(Graphics, step);

            node.GraphicalEventHandler.ContextClick += (sender, args) =>
            {
                TestableEditorElements.DisplayContextMenu(new List<TestableEditorElements.MenuOption>
                {
                    new TestableEditorElements.MenuItem(new GUIContent("Copy"), false, () =>
                    {
                        CopyStep(step);
                    }),
                    new TestableEditorElements.MenuItem(new GUIContent("Cut"), false, () =>
                    {
                        CutStep(step, node);
                    }),
                    new TestableEditorElements.MenuItem(new GUIContent("Delete"), false, () =>
                    {
                        DeleteStepWithUndo(step, node);
                    })
                });
            };

            node.GraphicalEventHandler.PointerDown += (sender, args) =>
            {
                UserSelectStepNode(node);
            };

            node.RelativePositionChanged += (sender, args) =>
            {
                node.Step.StepMetadata.Position = node.Position;
            };

            node.GraphicalEventHandler.PointerUp += (sender, args) =>
            {
                Graphics.CalculateBoundingBox();
            };

            // ReSharper disable once ImplicitlyCapturedClosure
            node.GraphicalEventHandler.PointerDown += (sender, args) => UserSelectStepNode(node);

            node.CreateTransitionButton.GraphicalEventHandler.PointerClick += (sender, args) =>
            {
                ITransition transition = new Transition();

                RevertableChangesHandler.Do(new TrainingCommand(
                    () =>
                    {
                        step.Data.Transitions.Data.Transitions.Add(transition);
                        MarkToRefresh();
                    },
                    () =>
                    {
                        step.Data.Transitions.Data.Transitions.Remove(transition);
                        MarkToRefresh();
                    }
                ));
            };

            if (currentChapter.ChapterMetadata.LastSelectedStep == step)
            {
                SelectStepNode(node);
            }

            SetupNode(node, position => node.Step.StepMetadata.Position = position);

            return node;
        }

        private void SelectStepNode(StepNode stepNode)
        {
            IStep step = stepNode == null ? null : stepNode.Step;

            if (lastSelectedStepNode != null)
            {
                lastSelectedStepNode.IsLastSelectedStep = false;
            }

            lastSelectedStepNode = stepNode;
            currentChapter.ChapterMetadata.LastSelectedStep = step;

            if (stepNode != null)
            {
                stepNode.IsLastSelectedStep = true;
            }
        }

        private void UserSelectStepNode(StepNode stepNode)
        {
            SelectStepNode(stepNode);
            Graphics.BringToTop(stepNode);

            if (stepNode != null)
            {
                StepWindow.ShowInspector();
            }
        }

        private void MarkToRefresh()
        {
            isUpdated = false;
        }

        private EntryNode CreateEntryNode(IChapter chapter)
        {
            EntryNode entryNode = new EntryNode(Graphics);

            ExitJoint joint = new ExitJoint(Graphics, entryNode)
            {
                RelativePosition = new Vector2(entryNode.LocalBoundingBox.xMax, entryNode.LocalBoundingBox.center.y),
            };

            entryNode.ExitJoints.Add(joint);

            entryNode.Position = chapter.ChapterMetadata.EntryNodePosition;

            entryNode.RelativePositionChanged += (sender, args) =>
            {
                chapter.ChapterMetadata.EntryNodePosition = entryNode.Position;
            };

            entryNode.GraphicalEventHandler.PointerUp += (sender, args) =>
            {
                Graphics.CalculateBoundingBox();
            };

            entryNode.GraphicalEventHandler.ContextClick += (sender, args) =>
            {
                if (chapter.Data.FirstStep == null)
                {
                    return;
                }

                TestableEditorElements.DisplayContextMenu(new List<TestableEditorElements.MenuOption>
                {
                    new TestableEditorElements.MenuItem(new GUIContent("Delete transition"), false, () =>
                    {
                        IStep firstStep = chapter.Data.FirstStep;

                        RevertableChangesHandler.Do(new TrainingCommand(() =>
                            {
                                chapter.Data.FirstStep = null;
                                MarkToRefresh();
                            },
                            () =>
                            {
                                chapter.Data.FirstStep = firstStep;
                                MarkToRefresh();
                            }
                        ));
                    })
                });
            };

            joint.GraphicalEventHandler.PointerDrag += (sender, args) =>
            {
                joint.DragDelta = args.PointerPosition - joint.Position;
            };

            joint.GraphicalEventHandler.PointerUp += (sender, args) =>
            {
                EntryJoint endJoint = Graphics.GetGraphicalElementWithHandlerAtPoint(args.PointerPosition).FirstOrDefault() as EntryJoint;

                if (endJoint == null)
                {
                    joint.DragDelta = Vector2.zero;
                    return;
                }

                StepNode endJointStepNode = endJoint.Parent as StepNode;

                IStep targetStep = null;
                IStep oldStep = chapter.Data.FirstStep;

                if (endJointStepNode != null)
                {
                    targetStep = endJointStepNode.Step;
                }

                RevertableChangesHandler.Do(new TrainingCommand(() =>
                    {
                        chapter.Data.FirstStep = targetStep;
                        MarkToRefresh();
                    },
                    () =>
                    {
                        chapter.Data.FirstStep = oldStep;
                        MarkToRefresh();
                    }
                ));

                joint.DragDelta = Vector2.zero;
            };

            SetupNode(entryNode, position => chapter.ChapterMetadata.EntryNodePosition = position);

            return entryNode;
        }

        private void SetupTransitions(IChapter chapter, EntryNode entryNode, IDictionary<IStep, StepNode> stepNodes)
        {
            if (chapter.Data.FirstStep != null)
            {
                CreateNewTransition(entryNode.ExitJoints.First(), stepNodes[chapter.Data.FirstStep].EntryJoints.First());
            }

            foreach (IStep step in stepNodes.Keys)
            {
                foreach (ITransition transition in step.Data.Transitions.Data.Transitions)
                {
                    ExitJoint joint = stepNodes[step].AddExitJoint();
                    if (transition.Data.TargetStep != null)
                    {
                        StepNode target = stepNodes[transition.Data.TargetStep];
                        CreateNewTransition(joint, target.EntryJoints.First());
                    }

                    IStep closuredStep = step;
                    ITransition closuredTransition = transition;
                    int transitionIndex = step.Data.Transitions.Data.Transitions.IndexOf(closuredTransition);

                    joint.GraphicalEventHandler.PointerDrag += (sender, args) =>
                    {
                        joint.DragDelta = args.PointerPosition - joint.Position;
                    };

                    joint.GraphicalEventHandler.PointerUp += (sender, args) =>
                    {
                        GraphicalElement elementUnderCursor = Graphics.GetGraphicalElementWithHandlerAtPoint(args.PointerPosition).FirstOrDefault();

                        EntryJoint endJoint = elementUnderCursor as EntryJoint;

                        if (endJoint == null)
                        {
                            joint.DragDelta = Vector2.zero;

                            if (elementUnderCursor != null)
                            {
                                return;
                            }
                        }

                        StepNode endJointStepNode = endJoint == null ? null : endJoint.Parent as StepNode;

                        IStep targetStep = null;
                        IStep oldStep = closuredTransition.Data.TargetStep;

                        if (endJointStepNode != null)
                        {
                            targetStep = endJointStepNode.Step;
                        }

                        RevertableChangesHandler.Do(new TrainingCommand(() =>
                            {
                                closuredTransition.Data.TargetStep = targetStep;
                                SelectStepNode(stepNodes[closuredStep]);
                                MarkToRefresh();
                            },
                            () =>
                            {
                                closuredTransition.Data.TargetStep = oldStep;
                                SelectStepNode(stepNodes[closuredStep]);
                                MarkToRefresh();
                            }
                        ));

                        joint.DragDelta = Vector2.zero;
                    };

                    joint.GraphicalEventHandler.ContextClick += (sender, args) =>
                    {
                        TestableEditorElements.DisplayContextMenu(new List<TestableEditorElements.MenuOption>
                        {
                            new TestableEditorElements.MenuItem(new GUIContent("Delete transition"), false, () =>
                            {
                                RevertableChangesHandler.Do(new TrainingCommand(() =>
                                    {
                                        closuredStep.Data.Transitions.Data.Transitions.Remove(closuredTransition);
                                        MarkToRefresh();
                                    },
                                    () =>
                                    {
                                        closuredStep.Data.Transitions.Data.Transitions.Insert(transitionIndex, closuredTransition);
                                        MarkToRefresh();
                                    }
                                ));
                            })
                        });
                    };
                }
            }
        }

        private IDictionary<IStep, StepNode> SetupSteps(IChapter chapter)
        {
            return chapter.Data.Steps.OrderBy(step => step == chapter.ChapterMetadata.LastSelectedStep).ToDictionary(step => step, CreateNewStepNode);
        }

        private void DeleteStep(IStep step)
        {
            if (currentChapter.ChapterMetadata.LastSelectedStep == step)
            {
                currentChapter.ChapterMetadata.LastSelectedStep = null;
            }

            currentChapter.Data.Steps.Remove(step);
            MarkToRefresh();
        }

        private void AddStep(IStep step)
        {
            currentChapter.Data.Steps.Add(step);

            MarkToRefresh();
        }

        private void CreateNewTransition(ExitJoint from, EntryJoint to)
        {
            TransitionElement transitionElement = new TransitionElement(Graphics, from, to);
            transitionElement.RelativePosition = Vector2.zero;
        }

        private void AddStepWithUndo(IStep step)
        {
            RevertableChangesHandler.Do(new TrainingCommand(() =>
                {
                    AddStep(step);
                    currentChapter.ChapterMetadata.LastSelectedStep = step;
                },
                () =>
                {
                    DeleteStep(step);
                }
            ));
        }

        private void HandleCanvasContextClick(object sender, PointerGraphicalElementEventArgs e)
        {
            IList<TestableEditorElements.MenuOption> options = new List<TestableEditorElements.MenuOption>();

            options.Add(new TestableEditorElements.MenuItem(new GUIContent("Add step"), false, () =>
            {
                IStep step = new Step("New Step");
                step.StepMetadata.Position = e.PointerPosition;

                AddStepWithUndo(step);
            }));

            if (SystemClipboard.IsStepInClipboard())
            {
                options.Add(new TestableEditorElements.MenuItem(new GUIContent("Paste step"), false, () =>
                {
                    Paste(e.PointerPosition);
                }));
            }
            else
            {
                options.Add(new TestableEditorElements.DisabledMenuItem(new GUIContent("Paste step")));
            }

            TestableEditorElements.DisplayContextMenu(options);
        }

        public void SetChapter(IChapter chapter)
        {
            currentChapter = chapter;

            Graphics.Reset();

            Graphics.Canvas.ContextClick += HandleCanvasContextClick;

            EntryNode entryNode = CreateEntryNode(chapter);
            IDictionary<IStep, StepNode> stepNodes = SetupSteps(chapter);
            SetupTransitions(chapter, entryNode, stepNodes);

            Graphics.CalculateBoundingBox();
        }

        public void HandleEvent(Event current, Rect controlRect)
        {
            if (isUpdated == false)
            {
                SetChapter(currentChapter);
                isUpdated = true;
            }

            Graphics.HandleEvent(current, controlRect);
        }

        private bool CopyStep(IStep step)
        {
            if (step == null)
            {
                return false;
            }

            try
            {
                SystemClipboard.CopyStep(step);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Copies the selected step into the system's copy buffer.
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool CopySelected()
        {
            IStep step = currentChapter.ChapterMetadata.LastSelectedStep;
            return CopyStep(step);
        }

        private bool CutStep(IStep step, StepNode owner)
        {
            if (CopyStep(step))
            {
                DeleteStepWithUndo(step, owner);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cuts the selected step into the system's copy buffer from the chapter.
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool CutSelected()
        {
            IStep step = currentChapter.ChapterMetadata.LastSelectedStep;
            return CutStep(step, lastSelectedStepNode);
        }

        /// <summary>
        /// Pastes the step from the system's copy buffer into the chapter at given <paramref name="position"/>.
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool Paste(Vector2 position)
        {
            IStep step;
            try
            {
                step = SystemClipboard.PasteStep();

                if (step == null)
                {
                    return false;
                }

                step.Data.Name = "Copy of " + step.Data.Name;

                step.StepMetadata.Position = position - new Vector2(0f, step.Data.Transitions.Data.Transitions.Count * 20f / 2f);
            }
            catch
            {
                return false;
            }

            AddStepWithUndo(step);

            return true;
        }

        /// <summary>
        /// Deletes the selected step from the chapter.
        /// </summary>
        /// <returns>True if successful.</returns>
        public bool DeleteSelected()
        {
            IStep step = currentChapter.ChapterMetadata.LastSelectedStep;
            if (step == null)
            {
                return false;
            }

            DeleteStepWithUndo(step, lastSelectedStepNode);
            return true;
        }
    }
}