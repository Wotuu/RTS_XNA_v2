namespace MapEditor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fIleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTilesetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PnlPaletteContainer = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.BtnShowGrid = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Layers = new System.Windows.Forms.ToolStripLabel();
            this.BtnLayerDown = new System.Windows.Forms.ToolStripButton();
            this.TBCurrentLayer = new System.Windows.Forms.ToolStripTextBox();
            this.BtnLayerUp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.BtnPaint = new System.Windows.Forms.ToolStripButton();
            this.BtnMarqueePaint = new System.Windows.Forms.ToolStripButton();
            this.BtnFill = new System.Windows.Forms.ToolStripButton();
            this.BtnErase = new System.Windows.Forms.ToolStripButton();
            this.BtnMarqueeerase = new System.Windows.Forms.ToolStripButton();
            this.tileMapDisplay1 = new MapEditor.Display.TileMapDisplay(this.components);
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.BtnDrawCollision = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fIleToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1264, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fIleToolStripMenuItem
            // 
            this.fIleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMapToolStripMenuItem,
            this.openTilesetToolStripMenuItem});
            this.fIleToolStripMenuItem.Name = "fIleToolStripMenuItem";
            this.fIleToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fIleToolStripMenuItem.Text = "File";
            // 
            // newMapToolStripMenuItem
            // 
            this.newMapToolStripMenuItem.Name = "newMapToolStripMenuItem";
            this.newMapToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.newMapToolStripMenuItem.Text = "New Map";
            this.newMapToolStripMenuItem.Click += new System.EventHandler(this.newMapToolStripMenuItem_Click);
            // 
            // openTilesetToolStripMenuItem
            // 
            this.openTilesetToolStripMenuItem.Name = "openTilesetToolStripMenuItem";
            this.openTilesetToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.openTilesetToolStripMenuItem.Text = "Open Tileset";
            this.openTilesetToolStripMenuItem.Click += new System.EventHandler(this.openTilesetToolStripMenuItem_Click);
            // 
            // PnlPaletteContainer
            // 
            this.PnlPaletteContainer.Location = new System.Drawing.Point(843, 55);
            this.PnlPaletteContainer.Name = "PnlPaletteContainer";
            this.PnlPaletteContainer.Size = new System.Drawing.Size(400, 640);
            this.PnlPaletteContainer.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnShowGrid,
            this.toolStripSeparator1,
            this.Layers,
            this.BtnLayerDown,
            this.TBCurrentLayer,
            this.BtnLayerUp,
            this.toolStripSeparator2,
            this.BtnPaint,
            this.BtnMarqueePaint,
            this.BtnFill,
            this.BtnErase,
            this.BtnMarqueeerase,
            this.toolStripSeparator3,
            this.BtnDrawCollision});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1264, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // BtnShowGrid
            // 
            this.BtnShowGrid.Checked = true;
            this.BtnShowGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BtnShowGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnShowGrid.Image = ((System.Drawing.Image)(resources.GetObject("BtnShowGrid.Image")));
            this.BtnShowGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnShowGrid.Name = "BtnShowGrid";
            this.BtnShowGrid.Size = new System.Drawing.Size(23, 22);
            this.BtnShowGrid.Text = "Toggle grid visibility";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // Layers
            // 
            this.Layers.Name = "Layers";
            this.Layers.Size = new System.Drawing.Size(40, 22);
            this.Layers.Text = "Layers";
            // 
            // BtnLayerDown
            // 
            this.BtnLayerDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnLayerDown.Image = ((System.Drawing.Image)(resources.GetObject("BtnLayerDown.Image")));
            this.BtnLayerDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnLayerDown.Name = "BtnLayerDown";
            this.BtnLayerDown.Size = new System.Drawing.Size(23, 22);
            this.BtnLayerDown.Text = "Layer Down";
            this.BtnLayerDown.Click += new System.EventHandler(this.BtnLayerDown_Click);
            // 
            // TBCurrentLayer
            // 
            this.TBCurrentLayer.MaxLength = 3;
            this.TBCurrentLayer.Name = "TBCurrentLayer";
            this.TBCurrentLayer.ReadOnly = true;
            this.TBCurrentLayer.Size = new System.Drawing.Size(25, 25);
            this.TBCurrentLayer.Text = "0";
            // 
            // BtnLayerUp
            // 
            this.BtnLayerUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnLayerUp.Image = ((System.Drawing.Image)(resources.GetObject("BtnLayerUp.Image")));
            this.BtnLayerUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnLayerUp.Name = "BtnLayerUp";
            this.BtnLayerUp.Size = new System.Drawing.Size(23, 22);
            this.BtnLayerUp.Text = "Layer Up";
            this.BtnLayerUp.Click += new System.EventHandler(this.BtnLayerUp_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // BtnPaint
            // 
            this.BtnPaint.Checked = true;
            this.BtnPaint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.BtnPaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnPaint.Image = ((System.Drawing.Image)(resources.GetObject("BtnPaint.Image")));
            this.BtnPaint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnPaint.Name = "BtnPaint";
            this.BtnPaint.Size = new System.Drawing.Size(23, 22);
            this.BtnPaint.Text = "Paint";
            this.BtnPaint.Click += new System.EventHandler(this.BtnPaint_Click);
            // 
            // BtnMarqueePaint
            // 
            this.BtnMarqueePaint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnMarqueePaint.Image = ((System.Drawing.Image)(resources.GetObject("BtnMarqueePaint.Image")));
            this.BtnMarqueePaint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnMarqueePaint.Name = "BtnMarqueePaint";
            this.BtnMarqueePaint.Size = new System.Drawing.Size(23, 22);
            this.BtnMarqueePaint.Text = "MarqueePaint";
            this.BtnMarqueePaint.Click += new System.EventHandler(this.BtnMarqueePaint_Click);
            // 
            // BtnFill
            // 
            this.BtnFill.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnFill.Image = ((System.Drawing.Image)(resources.GetObject("BtnFill.Image")));
            this.BtnFill.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnFill.Name = "BtnFill";
            this.BtnFill.Size = new System.Drawing.Size(23, 22);
            this.BtnFill.Text = "Fill Bucket";
            this.BtnFill.Click += new System.EventHandler(this.BtnFill_Click);
            // 
            // BtnErase
            // 
            this.BtnErase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnErase.Image = ((System.Drawing.Image)(resources.GetObject("BtnErase.Image")));
            this.BtnErase.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnErase.Name = "BtnErase";
            this.BtnErase.Size = new System.Drawing.Size(23, 22);
            this.BtnErase.Text = "Eraser";
            this.BtnErase.Click += new System.EventHandler(this.BtnErase_Click);
            // 
            // BtnMarqueeerase
            // 
            this.BtnMarqueeerase.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnMarqueeerase.Image = ((System.Drawing.Image)(resources.GetObject("BtnMarqueeerase.Image")));
            this.BtnMarqueeerase.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnMarqueeerase.Name = "BtnMarqueeerase";
            this.BtnMarqueeerase.Size = new System.Drawing.Size(23, 22);
            this.BtnMarqueeerase.Text = "Marquee Eraser";
            this.BtnMarqueeerase.Click += new System.EventHandler(this.BtnMarqueeerase_Click);
            // 
            // tileMapDisplay1
            // 
            this.tileMapDisplay1.Location = new System.Drawing.Point(12, 55);
            this.tileMapDisplay1.Name = "tileMapDisplay1";
            this.tileMapDisplay1.Size = new System.Drawing.Size(800, 640);
            this.tileMapDisplay1.TabIndex = 0;
            this.tileMapDisplay1.Text = "tileMapDisplay1";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // BtnDrawCollision
            // 
            this.BtnDrawCollision.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.BtnDrawCollision.Image = ((System.Drawing.Image)(resources.GetObject("BtnDrawCollision.Image")));
            this.BtnDrawCollision.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.BtnDrawCollision.Name = "BtnDrawCollision";
            this.BtnDrawCollision.Size = new System.Drawing.Size(23, 22);
            this.BtnDrawCollision.Text = "toolStripButton1";
            this.BtnDrawCollision.ToolTipText = "Draw Collision";
            this.BtnDrawCollision.Click += new System.EventHandler(this.BtnDrawCollision_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 710);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.PnlPaletteContainer);
            this.Controls.Add(this.tileMapDisplay1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "XNA Map Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Display.TileMapDisplay tileMapDisplay1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fIleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openTilesetToolStripMenuItem;
        private System.Windows.Forms.Panel PnlPaletteContainer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton BtnShowGrid;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel Layers;
        private System.Windows.Forms.ToolStripButton BtnLayerDown;
        private System.Windows.Forms.ToolStripTextBox TBCurrentLayer;
        private System.Windows.Forms.ToolStripButton BtnLayerUp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton BtnPaint;
        private System.Windows.Forms.ToolStripButton BtnMarqueePaint;
        private System.Windows.Forms.ToolStripButton BtnErase;
        private System.Windows.Forms.ToolStripButton BtnMarqueeerase;
        private System.Windows.Forms.ToolStripButton BtnFill;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton BtnDrawCollision;
    }
}

