using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace blmarket
{
    // This class demonstrates a simple custom layout panel.
    // It overrides the LayoutEngine property of the Panel
    // control to provide a custom layout engine. 
    public class DemoFlowPanel : Panel
    {
        private DemoFlowLayout layoutEngine;

        public DemoFlowPanel()
        {
        }

        public override LayoutEngine LayoutEngine
        {
            get
            {
                if (layoutEngine == null)
                {
                    layoutEngine = new DemoFlowLayout();
                }

                return layoutEngine;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return Size.Empty;
            }
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = Size.Empty;

            foreach (Control c in this.Controls)
            {
                if (c.Visible)
                    preferredSize.Height += c.Height + c.Margin.Bottom + c.Margin.Top;
            }

            return preferredSize;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }

    // This class demonstrates a simple custom layout engine.
    public class DemoFlowLayout : LayoutEngine
    {
        public override bool Layout(
            object container,
            LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            // Use DisplayRectangle so that parent.Padding is honored.
            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            Point nextControlLocation = parentDisplayRectangle.Location;

            for (int i = parent.Controls.Count - 1; i >= 0; i--)
            {
                Control c = parent.Controls[i];
                // Only apply layout to visible controls.
                if (!c.Visible)
                {
                    continue;
                }

                // Respect the margin of the control:
                // shift over the left and the top.
                nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);

                // Set the location of the control.
                c.Location = nextControlLocation;

                // Move X back to the display rectangle origin.
                nextControlLocation.X = parentDisplayRectangle.X;

                // Increment Y by the height of the control 
                // and the bottom margin.
                nextControlLocation.Y += c.Height + c.Margin.Bottom;
            }

            // Optional: Return whether or not the container's 
            // parent should perform layout as a result of this 
            // layout. Some layout engines return the value of 
            // the container's AutoSize property.
            return parent.AutoSize;
        }
    }
}