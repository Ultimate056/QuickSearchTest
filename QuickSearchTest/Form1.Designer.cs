namespace QuickSearchTest
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.gcUsku = new DevExpress.XtraGrid.GridControl();
            this.gvUsku = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gcToken = new DevExpress.XtraGrid.GridControl();
            this.gvToken = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SearchWordEdit = new DevExpress.XtraEditors.TextEdit();
            this.SearchButton = new DevExpress.XtraEditors.SimpleButton();
            this.gcResults = new DevExpress.XtraGrid.GridControl();
            this.gvResults = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.timeSearchLabel = new System.Windows.Forms.Label();
            this.searchControl1 = new DevExpress.XtraEditors.SearchControl();
            this.searchControl2 = new DevExpress.XtraEditors.SearchControl();
            this.gcMatches = new DevExpress.XtraGrid.GridControl();
            this.gvMatches = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gcUsku)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvUsku)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcToken)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvToken)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchWordEdit.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl2.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcMatches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMatches)).BeginInit();
            this.SuspendLayout();
            // 
            // gcUsku
            // 
            this.gcUsku.Location = new System.Drawing.Point(77, 61);
            this.gcUsku.MainView = this.gvUsku;
            this.gcUsku.Name = "gcUsku";
            this.gcUsku.Size = new System.Drawing.Size(400, 360);
            this.gcUsku.TabIndex = 18;
            this.gcUsku.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvUsku});
            // 
            // gvUsku
            // 
            this.gvUsku.GridControl = this.gcUsku;
            this.gvUsku.Name = "gvUsku";
            // 
            // gcToken
            // 
            this.gcToken.Location = new System.Drawing.Point(591, 61);
            this.gcToken.MainView = this.gvToken;
            this.gcToken.Name = "gcToken";
            this.gcToken.Size = new System.Drawing.Size(533, 360);
            this.gcToken.TabIndex = 19;
            this.gcToken.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvToken});
            // 
            // gvToken
            // 
            this.gvToken.GridControl = this.gcToken;
            this.gvToken.Name = "gvToken";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(817, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Токены по U-SKU";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(213, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "U-SKU";
            // 
            // SearchWordEdit
            // 
            this.SearchWordEdit.Location = new System.Drawing.Point(426, 452);
            this.SearchWordEdit.Name = "SearchWordEdit";
            this.SearchWordEdit.Properties.Appearance.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchWordEdit.Properties.Appearance.Options.UseFont = true;
            this.SearchWordEdit.Size = new System.Drawing.Size(224, 26);
            this.SearchWordEdit.TabIndex = 25;
            this.SearchWordEdit.EditValueChanged += new System.EventHandler(this.SearchWordEdit_EditValueChanged);
            this.SearchWordEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.SearchWordEdit_EditValueChanging);
            // 
            // SearchButton
            // 
            this.SearchButton.ImageOptions.Image = ((System.Drawing.Image)(resources.GetObject("SearchButton.ImageOptions.Image")));
            this.SearchButton.Location = new System.Drawing.Point(526, 484);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(39, 35);
            this.SearchButton.TabIndex = 26;
            this.SearchButton.Visible = false;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // gcResults
            // 
            this.gcResults.Location = new System.Drawing.Point(591, 542);
            this.gcResults.MainView = this.gvResults;
            this.gcResults.Name = "gcResults";
            this.gcResults.Size = new System.Drawing.Size(1053, 360);
            this.gcResults.TabIndex = 27;
            this.gcResults.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvResults});
            // 
            // gvResults
            // 
            this.gvResults.GridControl = this.gcResults;
            this.gvResults.Name = "gvResults";
            // 
            // timeSearchLabel
            // 
            this.timeSearchLabel.AutoSize = true;
            this.timeSearchLabel.Location = new System.Drawing.Point(40, 929);
            this.timeSearchLabel.Name = "timeSearchLabel";
            this.timeSearchLabel.Size = new System.Drawing.Size(85, 13);
            this.timeSearchLabel.TabIndex = 28;
            this.timeSearchLabel.Text = "Время поиска: ";
            // 
            // searchControl1
            // 
            this.searchControl1.Client = this.gcUsku;
            this.searchControl1.Location = new System.Drawing.Point(311, 35);
            this.searchControl1.Name = "searchControl1";
            this.searchControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this.searchControl1.Properties.Client = this.gcUsku;
            this.searchControl1.Size = new System.Drawing.Size(166, 20);
            this.searchControl1.TabIndex = 30;
            // 
            // searchControl2
            // 
            this.searchControl2.Client = this.gcResults;
            this.searchControl2.Location = new System.Drawing.Point(617, 516);
            this.searchControl2.Name = "searchControl2";
            this.searchControl2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this.searchControl2.Properties.Client = this.gcResults;
            this.searchControl2.Size = new System.Drawing.Size(166, 20);
            this.searchControl2.TabIndex = 31;
            // 
            // gcMatches
            // 
            this.gcMatches.Location = new System.Drawing.Point(32, 542);
            this.gcMatches.MainView = this.gvMatches;
            this.gcMatches.Name = "gcMatches";
            this.gcMatches.Size = new System.Drawing.Size(533, 360);
            this.gcMatches.TabIndex = 32;
            this.gcMatches.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvMatches});
            // 
            // gvMatches
            // 
            this.gvMatches.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3});
            this.gvMatches.GridControl = this.gcMatches;
            this.gvMatches.Name = "gvMatches";
            this.gvMatches.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvMatches_RowClick);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "idUSKU";
            this.gridColumn1.FieldName = "idUSKU";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 135;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Фильтр";
            this.gridColumn2.FieldName = "filter";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 143;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Совпадение";
            this.gridColumn3.FieldName = "nameMatch";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 0;
            this.gridColumn3.Width = 237;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 961);
            this.Controls.Add(this.gcMatches);
            this.Controls.Add(this.searchControl2);
            this.Controls.Add(this.searchControl1);
            this.Controls.Add(this.timeSearchLabel);
            this.Controls.Add(this.gcResults);
            this.Controls.Add(this.SearchButton);
            this.Controls.Add(this.SearchWordEdit);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gcToken);
            this.Controls.Add(this.gcUsku);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gcUsku)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvUsku)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcToken)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvToken)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SearchWordEdit.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl2.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gcMatches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvMatches)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DevExpress.XtraGrid.GridControl gcUsku;
        private DevExpress.XtraGrid.Views.Grid.GridView gvUsku;
        private DevExpress.XtraGrid.GridControl gcToken;
        private DevExpress.XtraGrid.Views.Grid.GridView gvToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.TextEdit SearchWordEdit;
        private DevExpress.XtraEditors.SimpleButton SearchButton;
        private DevExpress.XtraGrid.GridControl gcResults;
        private DevExpress.XtraGrid.Views.Grid.GridView gvResults;
        private System.Windows.Forms.Label timeSearchLabel;
        private DevExpress.XtraEditors.SearchControl searchControl1;
        private DevExpress.XtraEditors.SearchControl searchControl2;
        private DevExpress.XtraGrid.GridControl gcMatches;
        private DevExpress.XtraGrid.Views.Grid.GridView gvMatches;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
    }
}

