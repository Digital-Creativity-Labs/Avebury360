﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace CuttingRoom.Editor
{
    public class NavigationToolbar : EditorToobarBase
    {
        /// <summary>
        /// The back button which pops a view off the stack.
        /// </summary>
        private Button viewBackButton = null;

        /// <summary>
        /// The buttons in the current breadcrumb trail.
        /// </summary>
        private List<Button> viewContainerButtons = new List<Button>();

        /// <summary>
        /// The spacers in the current breadcrumb trail.
        /// </summary>
        private List<VisualElement> breadcrumbSpacers = new List<VisualElement>();

        /// <summary>
        /// Invoked whenever the back button is pressed on the navigation toolbar.
        /// </summary>
        public event Action OnClickViewBackButton;

        /// <summary>
        /// Invoked whenever one of the breadcrumbs is clicked.
        /// </summary>
        public event Action<ViewContainer> OnClickNavigationButton;

        public NavigationToolbar()
        {
            StyleSheet = Resources.Load<StyleSheet>("Toolbars/NavigationToolbar");
            styleSheets.Add(StyleSheet);
            name = "navigation-toolbar";
            AddViewBackButton();
        }

        private void AddViewBackButton()
        {
            viewBackButton = new Button(() => { OnClickViewBackButton?.Invoke(); });

            Image viewBackImage = new Image();

            // Add stylesheet.
            viewBackImage.styleSheets.Add(StyleSheet);

            // Apply name of class in stylesheet.
            viewBackImage.name = "back-image";

            Texture backIcon = Resources.Load<Texture>("Icons/back-icon-24x24");

            viewBackImage.image = backIcon;

            viewBackButton.Insert(0, viewBackImage);

            if (StyleSheet != null)
            {
                viewBackButton.styleSheets.Add(StyleSheet);
                viewBackButton.name = "toolbar-button";
            }

            Add(viewBackButton);
        }

        public void GenerateContents(Stack<ViewContainer> viewContainerStack)
        {
            // Remove existing breadcrumbs as these will be regenerated.
            foreach (Button button in viewContainerButtons)
            {
                Remove(button);
            }
            foreach (VisualElement spacer in breadcrumbSpacers)
            {
                Remove(spacer);
            }

            // Remove references to removed buttons and spacers.
            viewContainerButtons.Clear();
            breadcrumbSpacers.Clear();

            // Get the view containers in the view stack.
            List<ViewContainer> viewContainers = viewContainerStack.ToList();

            // Reverse this list to put the bottom of the stack at the start of the list.
            viewContainers.Reverse();

            NarrativeObject[] narrativeObjects = UnityEngine.Object.FindObjectsOfType<NarrativeObject>();

            // Add a breadcrumb for each view container.
            foreach (ViewContainer viewContainer in viewContainers)
            {
                Button button = new Button(() =>
                {
                    OnClickNavigationButton?.Invoke(viewContainer);
                });

                if (viewContainer.narrativeObjectGuid == EditorGraphView.rootViewContainerGuid)
                {
                    button.text = "Root";
                }
                else
                {
                    Label spacer = new Label();
                    spacer.text = ">";
                    spacer.name = "spacer";
                    Add(spacer);
                    breadcrumbSpacers.Add(spacer);

                    NarrativeObject narrativeObject = narrativeObjects.Where(narrativeObject => narrativeObject.guid == viewContainer.narrativeObjectGuid).FirstOrDefault();

                    if (narrativeObject != null)
                    {
                        button.text = narrativeObject.gameObject.name;
                    }
                }

                if (StyleSheet != null)
                {
                    button.styleSheets.Add(StyleSheet);
                    button.name = "toolbar-button";
                }

                Add(button);

                viewContainerButtons.Add(button);
            }

            // If the view stack has more than 1 view, then we can go back on the stack.
            if (viewContainerStack.Count > 1)
            {
                viewBackButton.SetEnabled(true);
            }
            else
            {
                viewBackButton.SetEnabled(false);
                
                // Disable the "Root" button as currently it does nothing as
                // the only view on the stack is the base root view.
                viewContainerButtons[0].SetEnabled(false);
            }
        }
    }
}
