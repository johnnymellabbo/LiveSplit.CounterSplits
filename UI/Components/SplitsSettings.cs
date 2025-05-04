using LiveSplit.Model;
using LiveSplit.TimeFormatters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using LiveSplit.Model.Input;
using System.Xml.Linq;

namespace LiveSplit.UI.Components
{
    public partial class SplitsSettings : UserControl
    {
        private int _VisualSplitCount { get; set; }
        public int VisualSplitCount
        {
            get { return _VisualSplitCount; }
            set
            {
                _VisualSplitCount = value;
                var max = Math.Max(0, _VisualSplitCount - (AlwaysShowLastSplit ? 2 : 1));
                if (dmnUpcomingSegments.Value > max)
                    dmnUpcomingSegments.Value = max;
                dmnUpcomingSegments.Maximum = max;
            }
        }
        public Color CurrentSplitTopColor { get; set; }
        public Color CurrentSplitBottomColor { get; set; }
        public int SplitPreviewCount { get; set; }
        public float SplitWidth { get; set; }
        public float SplitHeight { get; set; }
        public float ScaledSplitHeight { get { return SplitHeight * 10f; } set { SplitHeight = value / 10f; } }
        public float IconSize { get; set; }

        public bool Display2Rows { get; set; }

        public Color BackgroundColor { get; set; }
        public Color BackgroundColor2 { get; set; }

        public ExtendedGradientType BackgroundGradient { get; set; }
        public string GradientString
        {
            get { return BackgroundGradient.ToString(); }
            set { BackgroundGradient = (ExtendedGradientType)Enum.Parse(typeof(ExtendedGradientType), value); }
        }

        public LiveSplitState CurrentState { get; set; }

        public bool DisplayIcons { get; set; }
        public bool IconShadows { get; set; }
        public bool ShowThinSeparators { get; set; }
        public bool AlwaysShowLastSplit { get; set; }
        public bool ShowBlankSplits { get; set; }
        public bool LockLastSplit { get; set; }
        public bool SeparatorLastSplit { get; set; }

        public bool DropDecimals { get; set; }
        public TimeAccuracy DeltasAccuracy { get; set; }

        public bool OverrideDeltasColor { get; set; }
        public Color DeltasColor { get; set; }

        public bool ShowColumnLabels { get; set; }
        public Color LabelsColor { get; set; }

        public bool AutomaticAbbreviations { get; set; }
        public Color BeforeNamesColor { get; set; }
        public Color BeforeNamesColorLowCounter { get; set; }
        public Color BeforeNamesColorSameCounter { get; set; }
        public Color BeforeNamesColorHighCounter { get; set; }
        public Color CurrentNamesColor { get; set; }
        public Color CurrentNamesColorLowCounter { get; set; }
        public Color CurrentNamesColorSameCounter { get; set; }
        public Color CurrentNamesColorHighCounter { get; set; }
        public Color AfterNamesColor { get; set; }
        public bool OverrideTextColor { get; set; }
        public Color BeforeTimesColor { get; set; }
        public Color CurrentTimesColor { get; set; }
        public Color AfterTimesColor { get; set; }
        public bool OverrideTimesColor { get; set; }

        public bool ShowProgress { get; set; }
        public int ShowProgressTop { get; set; }
        public bool ShowProgressText { get; set; }
        public bool ShowProgressBar { get; set; }
        public bool ShowSplitCount { get; set; }
        public bool ShowPercentage { get; set; }
        public string ProgressText { get; set; }
        public int ShowProgressCurrentIndex { get; set; }

        public TimeAccuracy SplitTimesAccuracy { get; set; }
        public GradientType CurrentSplitGradient { get; set; }
        public string SplitGradientString { get { return CurrentSplitGradient.ToString(); } 
            set { CurrentSplitGradient = (GradientType)Enum.Parse(typeof(GradientType), value); } }

        public event EventHandler SplitLayoutChanged;

        public LayoutMode Mode { get; set; }

        public IList<ColumnSettings> ColumnsList { get; set; }
        public Size StartingSize { get; set; }
        public Size StartingTableLayoutSize { get; set; }

        public CompositeHook Hook { get; set; }
        public KeyOrButton IncrementKey { get; set; }
        public KeyOrButton DecrementKey { get; set; }
        public KeyOrButton ResetKey { get; set; }
        public KeyOrButton SaveKey { get; set; }


        public SplitsSettings(LiveSplitState state)
        {
            InitializeComponent();

            CurrentState = state;

            StartingSize = Size;
            StartingTableLayoutSize = tableColumns.Size;

            Hook = new CompositeHook();
            IncrementKey = new KeyOrButton(Keys.Add);
            DecrementKey = new KeyOrButton(Keys.Subtract);
            ResetKey = new KeyOrButton(Keys.Multiply);
            SaveKey = new KeyOrButton(Keys.Z);

            VisualSplitCount = 8;
            SplitPreviewCount = 1;
            DisplayIcons = true;
            IconShadows = true;
            ShowThinSeparators = true;
            AlwaysShowLastSplit = true;
            ShowBlankSplits = true;
            LockLastSplit = true;
            SeparatorLastSplit = true;
            SplitTimesAccuracy = TimeAccuracy.Seconds;
            CurrentSplitTopColor = Color.FromArgb(51, 115, 244);
            CurrentSplitBottomColor = Color.FromArgb(21, 53, 116);
            SplitWidth = 20;
            SplitHeight = 3.6f;
            IconSize = 24f;
            AutomaticAbbreviations = false;
            BeforeNamesColor = Color.FromArgb(255, 255, 255);
            BeforeNamesColorLowCounter = Color.FromArgb(255, 255, 255);
            BeforeNamesColorSameCounter = Color.FromArgb(255, 255, 255);
            BeforeNamesColorHighCounter = Color.FromArgb(255, 255, 255);
            CurrentNamesColor = Color.FromArgb(255, 255, 255);
            CurrentNamesColorLowCounter = Color.FromArgb(255, 255, 255);
            CurrentNamesColorSameCounter = Color.FromArgb(255, 255, 255);
            CurrentNamesColorHighCounter = Color.FromArgb(255, 255, 255);
            AfterNamesColor = Color.FromArgb(255, 255, 255);
            OverrideTextColor = false;
            BeforeTimesColor = Color.FromArgb(255, 255, 255);
            CurrentTimesColor = Color.FromArgb(255, 255, 255);
            AfterTimesColor = Color.FromArgb(255, 255, 255);
            OverrideTimesColor = false;
            CurrentSplitGradient = GradientType.Vertical;
            cmbSplitGradient.SelectedIndexChanged += cmbSplitGradient_SelectedIndexChanged;
            BackgroundColor = Color.Transparent;
            BackgroundColor2 = Color.FromArgb(1, 255, 255, 255);
            BackgroundGradient = ExtendedGradientType.Alternating;
            DropDecimals = true;
            DeltasAccuracy = TimeAccuracy.Tenths;
            OverrideDeltasColor = false;
            DeltasColor = Color.FromArgb(255, 255, 255);
            Display2Rows = false;
            ShowColumnLabels = false;
            LabelsColor = Color.FromArgb(255, 255, 255);

            ShowProgress = false;
            ShowProgressTop = 0;
            ShowProgressText = true;
            ShowProgressBar = true;
            ShowSplitCount = true;
            ShowPercentage = false;
            ProgressText = "Progress:";
            ShowProgressCurrentIndex = 0;

            txtIncrement.Text = FormatKey(IncrementKey);
            txtDecrement.Text = FormatKey(DecrementKey);
            txtReset.Text = FormatKey(ResetKey);
            txtSave.Text = FormatKey(SaveKey);
            dmnTotalSegments.DataBindings.Add("Value", this, "VisualSplitCount", false, DataSourceUpdateMode.OnPropertyChanged);
            dmnUpcomingSegments.DataBindings.Add("Value", this, "SplitPreviewCount", false, DataSourceUpdateMode.OnPropertyChanged);
            btnTopColor.DataBindings.Add("BackColor", this, "CurrentSplitTopColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBottomColor.DataBindings.Add("BackColor", this, "CurrentSplitBottomColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAutomaticAbbreviations.DataBindings.Add("Checked", this, "AutomaticAbbreviations", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeNamesColor.DataBindings.Add("BackColor", this, "BeforeNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeNamesColorLowCounter.DataBindings.Add("BackColor", this, "BeforeNamesColorLowCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeNamesColorSameCounter.DataBindings.Add("BackColor", this, "BeforeNamesColorSameCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeNamesColorHighCounter.DataBindings.Add("BackColor", this, "BeforeNamesColorHighCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentNamesColor.DataBindings.Add("BackColor", this, "CurrentNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentNamesColorLowCounter.DataBindings.Add("BackColor", this, "CurrentNamesColorLowCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentNamesColorSameCounter.DataBindings.Add("BackColor", this, "CurrentNamesColorSameCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentNamesColorHighCounter.DataBindings.Add("BackColor", this, "CurrentNamesColorHighCounter", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAfterNamesColor.DataBindings.Add("BackColor", this, "AfterNamesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnBeforeTimesColor.DataBindings.Add("BackColor", this, "BeforeTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnCurrentTimesColor.DataBindings.Add("BackColor", this, "CurrentTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnAfterTimesColor.DataBindings.Add("BackColor", this, "AfterTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDisplayIcons.DataBindings.Add("Checked", this, "DisplayIcons", false, DataSourceUpdateMode.OnPropertyChanged);
            chkIconShadows.DataBindings.Add("Checked", this, "IconShadows", false, DataSourceUpdateMode.OnPropertyChanged);
            chkThinSeparators.DataBindings.Add("Checked", this, "ShowThinSeparators", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLastSplit.DataBindings.Add("Checked", this, "AlwaysShowLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTextColor.DataBindings.Add("Checked", this, "OverrideTextColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideTimesColor.DataBindings.Add("Checked", this, "OverrideTimesColor", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowBlankSplits.DataBindings.Add("Checked", this, "ShowBlankSplits", false, DataSourceUpdateMode.OnPropertyChanged);
            chkLockLastSplit.DataBindings.Add("Checked", this, "LockLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSeparatorLastSplit.DataBindings.Add("Checked", this, "SeparatorLastSplit", false, DataSourceUpdateMode.OnPropertyChanged);
            chkDropDecimals.DataBindings.Add("Checked", this, "DropDecimals", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOverrideDeltaColor.DataBindings.Add("Checked", this, "OverrideDeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnDeltaColor.DataBindings.Add("BackColor", this, "DeltasColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnLabelColor.DataBindings.Add("BackColor", this, "LabelsColor", false, DataSourceUpdateMode.OnPropertyChanged);
            trkIconSize.DataBindings.Add("Value", this, "IconSize", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbSplitGradient.DataBindings.Add("SelectedItem", this, "SplitGradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            cmbGradientType.DataBindings.Add("SelectedItem", this, "GradientString", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor1.DataBindings.Add("BackColor", this, "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            btnColor2.DataBindings.Add("BackColor", this, "BackgroundColor2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkShowProgress.DataBindings.Add("Checked", this, "ShowProgress", false, DataSourceUpdateMode.OnPropertyChanged);
            chkText.DataBindings.Add("Checked", this, "ShowProgressText", false, DataSourceUpdateMode.OnPropertyChanged);
            chkProgressBar.DataBindings.Add("Checked", this, "ShowProgressBar", false, DataSourceUpdateMode.OnPropertyChanged);
            chkSplitCount.DataBindings.Add("Checked", this, "ShowSplitCount", false, DataSourceUpdateMode.OnPropertyChanged);
            chkPercentage.DataBindings.Add("Checked", this, "ShowPercentage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtText.DataBindings.Add("Text", this, "ProgressText", false, DataSourceUpdateMode.OnPropertyChanged);

            rdoTop.CheckedChanged += UpdateShowProgressTop;
            rdoMiddle.CheckedChanged += UpdateShowProgressTop;
            rdoBottom.CheckedChanged += UpdateShowProgressTop;
            InitializeShowProgressTop();

            rdoShowProgressCurrentIndex.CheckedChanged += UpdateShowProgressCurrentIndex;
            rdoShowProgressNextIndex.CheckedChanged += UpdateShowProgressCurrentIndex;
            InitializeShowProgressCurrentIndex();

            ColumnsList = new List<ColumnSettings>();
            ColumnsList.Add(new ColumnSettings(CurrentState, "Counter", ColumnsList) { Data = new ColumnData("Counter", ColumnType.Counter, "Current Comparison", "Current Timing Method") });

            RegisterHotKeys();
        }

        void chkColumnLabels_CheckedChanged(object sender, EventArgs e)
        {
            btnLabelColor.Enabled = lblLabelsColor.Enabled = chkColumnLabels.Checked;
        }

        void chkDisplayIcons_CheckedChanged(object sender, EventArgs e)
        {
            trkIconSize.Enabled = label5.Enabled = chkIconShadows.Enabled = chkDisplayIcons.Checked;
        }

        void chkOverrideTimesColor_CheckedChanged(object sender, EventArgs e)
        {
            label6.Enabled = label9.Enabled = label7.Enabled = btnBeforeTimesColor.Enabled
                = btnCurrentTimesColor.Enabled = btnAfterTimesColor.Enabled = chkOverrideTimesColor.Checked;
        }

        void chkOverrideDeltaColor_CheckedChanged(object sender, EventArgs e)
        {
            label8.Enabled = btnDeltaColor.Enabled = chkOverrideDeltaColor.Checked;
        }

        void chkOverrideTextColor_CheckedChanged(object sender, EventArgs e)
        {
            label3.Enabled = label10.Enabled = label13.Enabled = btnBeforeNamesColor.Enabled
            = btnCurrentNamesColor.Enabled = btnAfterNamesColor.Enabled
            = labelZero.Enabled = labelLow.Enabled = labelSame.Enabled = labelHigh.Enabled
            = btnBeforeNamesColorLowCounter.Enabled = btnBeforeNamesColorSameCounter.Enabled
            = btnBeforeNamesColorHighCounter.Enabled = btnCurrentNamesColorLowCounter.Enabled
            = btnCurrentNamesColorSameCounter.Enabled = btnCurrentNamesColorHighCounter.Enabled
            = chkOverrideTextColor.Checked;
        }

        // Event to enable/disable controls for "Show Progress" table.
        private void chkShowProgress_CheckedChanged(object sender, EventArgs e)
        {
            bool isEnabled = chkShowProgress.Checked;

            // Habilitar o deshabilitar los controles según el estado del checkbox principal
            rdoTop.Enabled = isEnabled;
            rdoMiddle.Enabled = isEnabled;
            rdoBottom.Enabled = isEnabled;
            chkText.Enabled = isEnabled;
            chkProgressBar.Enabled = isEnabled;
            chkSplitCount.Enabled = isEnabled;
            chkPercentage.Enabled = isEnabled;
            lblText.Enabled = isEnabled && chkText.Checked;
            txtText.Enabled = isEnabled && chkText.Checked;
            rdoShowProgressCurrentIndex.Enabled = isEnabled;
            rdoShowProgressNextIndex.Enabled = isEnabled;
            ShowProgress = chkShowProgress.Checked;
            SplitLayoutChanged(this, null);
        }

        // Event to enable/disable text field depending on "Text" checkbox.
        private void chkText_CheckedChanged(object sender, EventArgs e)
        {
            lblText.Enabled = chkText.Checked;
            txtText.Enabled = chkText.Checked;
            ShowProgressText = chkText.Checked;
            SplitLayoutChanged(this, null);
        }

        void rdoDeltaTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        void rdoDeltaSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDeltaAccuracy();
        }

        void chkSeparatorLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            SeparatorLastSplit = chkSeparatorLastSplit.Checked;
            SplitLayoutChanged(this, null);
        }

        void cmbGradientType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnColor1.Visible = cmbGradientType.SelectedItem.ToString() != "Plain";
            btnColor2.DataBindings.Clear();
            btnColor2.DataBindings.Add("BackColor", this, btnColor1.Visible ? "BackgroundColor2" : "BackgroundColor", false, DataSourceUpdateMode.OnPropertyChanged);
            GradientString = cmbGradientType.SelectedItem.ToString();
        }

        void cmbSplitGradient_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnTopColor.Visible = cmbSplitGradient.SelectedItem.ToString() != "Plain";
            btnBottomColor.DataBindings.Clear();
            btnBottomColor.DataBindings.Add("BackColor", this, btnTopColor.Visible ? "CurrentSplitBottomColor" : "CurrentSplitTopCOlor", false, DataSourceUpdateMode.OnPropertyChanged);
            SplitGradientString = cmbSplitGradient.SelectedItem.ToString();
        }

        void chkLockLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            LockLastSplit = chkLockLastSplit.Checked;
            SplitLayoutChanged(this, null);
        }

        void chkShowBlankSplits_CheckedChanged(object sender, EventArgs e)
        {
            ShowBlankSplits = chkLockLastSplit.Enabled = chkShowBlankSplits.Checked;
            SplitLayoutChanged(this, null);
        }

        void rdoTenths_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        void rdoSeconds_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAccuracy();
        }

        void UpdateAccuracy()
        {
            if (rdoSeconds.Checked)
                SplitTimesAccuracy = TimeAccuracy.Seconds;
            else if (rdoTenths.Checked)
                SplitTimesAccuracy = TimeAccuracy.Tenths;
            else
                SplitTimesAccuracy = TimeAccuracy.Hundredths;
        }

        void UpdateDeltaAccuracy()
        {
            if (rdoDeltaSeconds.Checked)
                DeltasAccuracy = TimeAccuracy.Seconds;
            else if (rdoDeltaTenths.Checked)
                DeltasAccuracy = TimeAccuracy.Tenths;
            else
                DeltasAccuracy = TimeAccuracy.Hundredths;
        }

        void chkLastSplit_CheckedChanged(object sender, EventArgs e)
        {
            AlwaysShowLastSplit = chkLastSplit.Checked;
            VisualSplitCount = VisualSplitCount;
            SplitLayoutChanged(this, null);
        }

        void chkThinSeparators_CheckedChanged(object sender, EventArgs e)
        {
            ShowThinSeparators = chkThinSeparators.Checked;
            SplitLayoutChanged(this, null);
        }
        private void UpdateShowProgressTop(object sender, EventArgs e)
        {
            if (rdoTop.Checked)
                ShowProgressTop = 0;
            else if (rdoMiddle.Checked)
                ShowProgressTop = 1;
            else if (rdoBottom.Checked)
                ShowProgressTop = 2;
        }

        private void InitializeShowProgressTop()
        {
            if (ShowProgressTop == 0)
                rdoTop.Checked = true;
            else if (ShowProgressTop == 1)
                rdoMiddle.Checked = true;
            else if (ShowProgressTop == 2)
                rdoBottom.Checked = true;
        }
        private void UpdateShowProgressCurrentIndex(object sender, EventArgs e)
        {
           if (rdoShowProgressCurrentIndex.Checked)
                ShowProgressCurrentIndex = 0;
            else if (rdoShowProgressNextIndex.Checked)
                ShowProgressCurrentIndex = 1;
        }
        private void InitializeShowProgressCurrentIndex()
        {
            if (ShowProgressCurrentIndex == 0)
                rdoShowProgressCurrentIndex.Checked = true;
            else if (ShowProgressCurrentIndex == 1)
                rdoShowProgressNextIndex.Checked = true;
        }
        

        void SplitsSettings_Load(object sender, EventArgs e)
        {
            ResetColumns();

            chkOverrideDeltaColor_CheckedChanged(null, null);
            chkOverrideTextColor_CheckedChanged(null, null);
            chkOverrideTimesColor_CheckedChanged(null, null);
            chkColumnLabels_CheckedChanged(null, null);
            chkDisplayIcons_CheckedChanged(null, null);
            chkText_CheckedChanged(null, null);
            chkShowProgress_CheckedChanged(null, null);

            chkLockLastSplit.Enabled = chkShowBlankSplits.Checked;

            rdoSeconds.Checked = SplitTimesAccuracy == TimeAccuracy.Seconds;
            rdoTenths.Checked = SplitTimesAccuracy == TimeAccuracy.Tenths;
            rdoHundredths.Checked = SplitTimesAccuracy == TimeAccuracy.Hundredths;

            rdoDeltaSeconds.Checked = DeltasAccuracy == TimeAccuracy.Seconds;
            rdoDeltaTenths.Checked = DeltasAccuracy == TimeAccuracy.Tenths;
            rdoDeltaHundredths.Checked = DeltasAccuracy == TimeAccuracy.Hundredths;

            if (Mode == LayoutMode.Horizontal)
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 5;
                trkSize.Maximum = 120;
                SplitWidth = Math.Min(Math.Max(trkSize.Minimum, SplitWidth), trkSize.Maximum);
                trkSize.DataBindings.Add("Value", this, "SplitWidth", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSplitSize.Text = "Split Width:";
                chkDisplayRows.Enabled = false;
                chkDisplayRows.DataBindings.Clear();
                chkDisplayRows.Checked = true;
                chkColumnLabels.DataBindings.Clear();
                chkColumnLabels.Enabled = chkColumnLabels.Checked = false;
            }
            else
            {
                trkSize.DataBindings.Clear();
                trkSize.Minimum = 0;
                trkSize.Maximum = 250;
                ScaledSplitHeight = Math.Min(Math.Max(trkSize.Minimum, ScaledSplitHeight), trkSize.Maximum);
                trkSize.DataBindings.Add("Value", this, "ScaledSplitHeight", false, DataSourceUpdateMode.OnPropertyChanged);
                lblSplitSize.Text = "Split Height:";
                chkDisplayRows.Enabled = true;
                chkDisplayRows.DataBindings.Clear();
                chkDisplayRows.DataBindings.Add("Checked", this, "Display2Rows", false, DataSourceUpdateMode.OnPropertyChanged);
                chkColumnLabels.DataBindings.Clear();
                chkColumnLabels.Enabled = true;
                chkColumnLabels.DataBindings.Add("Checked", this, "ShowColumnLabels", false, DataSourceUpdateMode.OnPropertyChanged);
            }

            // Inicializar el estado de los RadioButton según ShowProgressTop
            if (ShowProgressTop == 0)
                rdoTop.Checked = true;
            else if (ShowProgressTop == 1)
                rdoMiddle.Checked = true;
            else if (ShowProgressTop == 2)
                rdoBottom.Checked = true;

            if (ShowProgressCurrentIndex == 0)
                rdoShowProgressCurrentIndex.Checked = true;
            else if (ShowProgressCurrentIndex == 1)
                rdoShowProgressNextIndex.Checked = true;
        }

        public void SetSettings(XmlNode node)
        {
            var element = (XmlElement)node;
            Version version = SettingsHelper.ParseVersion(element["Version"]);

            CurrentSplitTopColor = SettingsHelper.ParseColor(element["CurrentSplitTopColor"]);
            CurrentSplitBottomColor = SettingsHelper.ParseColor(element["CurrentSplitBottomColor"]);
            VisualSplitCount = SettingsHelper.ParseInt(element["VisualSplitCount"]);
            SplitPreviewCount = SettingsHelper.ParseInt(element["SplitPreviewCount"]);
            DisplayIcons = SettingsHelper.ParseBool(element["DisplayIcons"]);
            ShowThinSeparators = SettingsHelper.ParseBool(element["ShowThinSeparators"]);
            AlwaysShowLastSplit = SettingsHelper.ParseBool(element["AlwaysShowLastSplit"]);
            SplitWidth = SettingsHelper.ParseFloat(element["SplitWidth"]);
            AutomaticAbbreviations = SettingsHelper.ParseBool(element["AutomaticAbbreviations"], false);
            ShowColumnLabels = SettingsHelper.ParseBool(element["ShowColumnLabels"], false);
            LabelsColor = SettingsHelper.ParseColor(element["LabelsColor"], Color.FromArgb(255, 255, 255));
            OverrideTimesColor = SettingsHelper.ParseBool(element["OverrideTimesColor"], false);
            BeforeTimesColor = SettingsHelper.ParseColor(element["BeforeTimesColor"], Color.FromArgb(255, 255, 255));
            CurrentTimesColor = SettingsHelper.ParseColor(element["CurrentTimesColor"], Color.FromArgb(255, 255, 255));
            AfterTimesColor = SettingsHelper.ParseColor(element["AfterTimesColor"], Color.FromArgb(255, 255, 255));
            SplitHeight = SettingsHelper.ParseFloat(element["SplitHeight"], 6);
            SplitGradientString = SettingsHelper.ParseString(element["CurrentSplitGradient"], GradientType.Vertical.ToString());
            BackgroundColor = SettingsHelper.ParseColor(element["BackgroundColor"], Color.Transparent);
            BackgroundColor2 = SettingsHelper.ParseColor(element["BackgroundColor2"], Color.Transparent);
            GradientString = SettingsHelper.ParseString(element["BackgroundGradient"], ExtendedGradientType.Plain.ToString());
            SeparatorLastSplit = SettingsHelper.ParseBool(element["SeparatorLastSplit"], true);
            DropDecimals = SettingsHelper.ParseBool(element["DropDecimals"], true);
            DeltasAccuracy = SettingsHelper.ParseEnum(element["DeltasAccuracy"], TimeAccuracy.Tenths);
            OverrideDeltasColor = SettingsHelper.ParseBool(element["OverrideDeltasColor"], false);
            DeltasColor = SettingsHelper.ParseColor(element["DeltasColor"], Color.FromArgb(255, 255, 255));
            Display2Rows = SettingsHelper.ParseBool(element["Display2Rows"], false);
            SplitTimesAccuracy = SettingsHelper.ParseEnum(element["SplitTimesAccuracy"], TimeAccuracy.Seconds);
            ShowBlankSplits = SettingsHelper.ParseBool(element["ShowBlankSplits"], true);
            LockLastSplit = SettingsHelper.ParseBool(element["LockLastSplit"], false);
            IconSize = SettingsHelper.ParseFloat(element["IconSize"], 24f);
            IconShadows = SettingsHelper.ParseBool(element["IconShadows"], true);

            if (version >= new Version(1, 5))
            {
                var columnsElement = element["Columns"];
                ColumnsList.Clear();
                foreach (var child in columnsElement.ChildNodes)
                {
                    var columnData = ColumnData.FromXml((XmlNode)child);
                    ColumnsList.Add(new ColumnSettings(CurrentState, columnData.Name, ColumnsList) { Data = columnData });
                }
            }
            else
            {
                ColumnsList.Clear();
                var comparison = SettingsHelper.ParseString(element["Comparison"]);
                if (SettingsHelper.ParseBool(element["ShowSplitTimes"]))
                {
                    ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.Delta, comparison, "Current Timing Method")});
                    ColumnsList.Add(new ColumnSettings(CurrentState, "Time", ColumnsList) { Data = new ColumnData("Time", ColumnType.SplitTime, comparison, "Current Timing Method")});
                }
                else
                {
                    ColumnsList.Add(new ColumnSettings(CurrentState, "+/-", ColumnsList) { Data = new ColumnData("+/-", ColumnType.DeltaorSplitTime, comparison, "Current Timing Method") });
                }
            }
            if (version >= new Version(1, 3))
            {
                BeforeNamesColor = SettingsHelper.ParseColor(element["BeforeNamesColor"]);
                BeforeNamesColorLowCounter = SettingsHelper.ParseColor(element["BeforeNamesColorLowCounter"]);
                BeforeNamesColorSameCounter = SettingsHelper.ParseColor(element["BeforeNamesColorSameCounter"]);
                BeforeNamesColorHighCounter = SettingsHelper.ParseColor(element["BeforeNamesColorHighCounter"]);
                CurrentNamesColor = SettingsHelper.ParseColor(element["CurrentNamesColor"]);
                CurrentNamesColorLowCounter = SettingsHelper.ParseColor(element["CurrentNamesColorLowCounter"]);
                CurrentNamesColorSameCounter = SettingsHelper.ParseColor(element["CurrentNamesColorSameCounter"]);
                CurrentNamesColorHighCounter = SettingsHelper.ParseColor(element["CurrentNamesColorHighCounter"]);
                AfterNamesColor = SettingsHelper.ParseColor(element["AfterNamesColor"]);
                OverrideTextColor = SettingsHelper.ParseBool(element["OverrideTextColor"]);
            }
            else
            {
                if (version >= new Version(1, 2))
                    BeforeNamesColor = CurrentNamesColor = AfterNamesColor
                        = BeforeNamesColorLowCounter = BeforeNamesColorSameCounter = BeforeNamesColorHighCounter
                        = CurrentNamesColorLowCounter = CurrentNamesColorSameCounter = CurrentNamesColorHighCounter
                        = SettingsHelper.ParseColor(element["SplitNamesColor"]);
                else
                {
                    BeforeNamesColor = Color.FromArgb(255, 255, 255);
                    BeforeNamesColorLowCounter = Color.FromArgb(255, 255, 255);
                    BeforeNamesColorSameCounter = Color.FromArgb(255, 255, 255);
                    BeforeNamesColorHighCounter = Color.FromArgb(255, 255, 255);
                    CurrentNamesColor = Color.FromArgb(255, 255, 255);
                    CurrentNamesColorLowCounter = Color.FromArgb(255, 255, 255);
                    CurrentNamesColorSameCounter = Color.FromArgb(255, 255, 255);
                    CurrentNamesColorHighCounter = Color.FromArgb(255, 255, 255);
                    AfterNamesColor = Color.FromArgb(255, 255, 255);
                }
                OverrideTextColor = !SettingsHelper.ParseBool(element["UseTextColor"], true);
            }
            if (version >= new Version(1, 4))
            {
                ShowProgress = SettingsHelper.ParseBool(element["ShowProgress"]);
                ShowProgressTop = SettingsHelper.ParseInt(element["ShowProgressTop"], 0);
                ShowProgressText = SettingsHelper.ParseBool(element["ShowProgressText"]);
                ShowProgressBar = SettingsHelper.ParseBool(element["ShowProgressBar"]);
                ShowSplitCount = SettingsHelper.ParseBool(element["ShowSplitCount"]);
                ShowPercentage = SettingsHelper.ParseBool(element["ShowPercentage"]);
                ProgressText = SettingsHelper.ParseString(element["ProgressText"], "Progress:");
                ShowProgressCurrentIndex = SettingsHelper.ParseInt(element["ShowProgressCurrentIndex"], 0);
            }
            else
            {
                ShowProgress = false;
                ShowProgressTop = 0;
                ShowProgressText = true;
                ShowProgressBar = true;
                ShowSplitCount = true;
                ShowPercentage = false;
                ProgressText = "Progress:";
                ShowProgressCurrentIndex = 0;
            }

            XmlElement incrementElement = element["IncrementKey"];
            IncrementKey = string.IsNullOrEmpty(incrementElement.InnerText) ? null : new KeyOrButton(incrementElement.InnerText);
            XmlElement decrementElement = element["DecrementKey"];
            DecrementKey = string.IsNullOrEmpty(decrementElement.InnerText) ? null : new KeyOrButton(decrementElement.InnerText);
            XmlElement resetElement = element["ResetKey"];
            ResetKey = string.IsNullOrEmpty(resetElement.InnerText) ? null : new KeyOrButton(resetElement.InnerText);
            XmlElement saveElement = element["SaveKey"];
            SaveKey = string.IsNullOrEmpty(saveElement.InnerText) ? null : new KeyOrButton(saveElement.InnerText);


            txtIncrement.Text = FormatKey(IncrementKey);
            txtDecrement.Text = FormatKey(DecrementKey);
            txtReset.Text = FormatKey(ResetKey);
            txtSave.Text = FormatKey(SaveKey);

            RegisterHotKeys();
        }

        public XmlNode GetSettings(XmlDocument document)
        {
            var parent = document.CreateElement("Settings");
            CreateSettingsNode(document, parent);
            return parent;
        }

        public int GetSettingsHashCode()
        {
            return CreateSettingsNode(null, null);
        }

        private int CreateSettingsNode(XmlDocument document, XmlElement parent)
        {
            var hashCode = SettingsHelper.CreateSetting(document, parent, "Version", "1.6") ^
            SettingsHelper.CreateSetting(document, parent, "CurrentSplitTopColor", CurrentSplitTopColor) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentSplitBottomColor", CurrentSplitBottomColor) ^
            SettingsHelper.CreateSetting(document, parent, "VisualSplitCount", VisualSplitCount) ^
            SettingsHelper.CreateSetting(document, parent, "SplitPreviewCount", SplitPreviewCount) ^
            SettingsHelper.CreateSetting(document, parent, "DisplayIcons", DisplayIcons) ^
            SettingsHelper.CreateSetting(document, parent, "ShowThinSeparators", ShowThinSeparators) ^
            SettingsHelper.CreateSetting(document, parent, "AlwaysShowLastSplit", AlwaysShowLastSplit) ^
            SettingsHelper.CreateSetting(document, parent, "SplitWidth", SplitWidth) ^
            SettingsHelper.CreateSetting(document, parent, "SplitTimesAccuracy", SplitTimesAccuracy) ^
            SettingsHelper.CreateSetting(document, parent, "AutomaticAbbreviations", AutomaticAbbreviations) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeNamesColor", BeforeNamesColor) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeNamesColorLowCounter", BeforeNamesColorLowCounter) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeNamesColorSameCounter", BeforeNamesColorSameCounter) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeNamesColorHighCounter", BeforeNamesColorHighCounter) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentNamesColor", CurrentNamesColor) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentNamesColorLowCounter", CurrentNamesColorLowCounter) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentNamesColorSameCounter", CurrentNamesColorSameCounter) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentNamesColorHighCounter", CurrentNamesColorHighCounter) ^
            SettingsHelper.CreateSetting(document, parent, "AfterNamesColor", AfterNamesColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTextColor", OverrideTextColor) ^
            SettingsHelper.CreateSetting(document, parent, "BeforeTimesColor", BeforeTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentTimesColor", CurrentTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "AfterTimesColor", AfterTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideTimesColor", OverrideTimesColor) ^
            SettingsHelper.CreateSetting(document, parent, "ShowBlankSplits", ShowBlankSplits) ^
            SettingsHelper.CreateSetting(document, parent, "LockLastSplit", LockLastSplit) ^
            SettingsHelper.CreateSetting(document, parent, "IconSize", IconSize) ^
            SettingsHelper.CreateSetting(document, parent, "IconShadows", IconShadows) ^
            SettingsHelper.CreateSetting(document, parent, "SplitHeight", SplitHeight) ^
            SettingsHelper.CreateSetting(document, parent, "CurrentSplitGradient", CurrentSplitGradient) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor", BackgroundColor) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundColor2", BackgroundColor2) ^
            SettingsHelper.CreateSetting(document, parent, "BackgroundGradient", BackgroundGradient) ^
            SettingsHelper.CreateSetting(document, parent, "SeparatorLastSplit", SeparatorLastSplit) ^
            SettingsHelper.CreateSetting(document, parent, "DeltasAccuracy", DeltasAccuracy) ^
            SettingsHelper.CreateSetting(document, parent, "DropDecimals", DropDecimals) ^
            SettingsHelper.CreateSetting(document, parent, "OverrideDeltasColor", OverrideDeltasColor) ^
            SettingsHelper.CreateSetting(document, parent, "DeltasColor", DeltasColor) ^
            SettingsHelper.CreateSetting(document, parent, "Display2Rows", Display2Rows) ^
            SettingsHelper.CreateSetting(document, parent, "ShowColumnLabels", ShowColumnLabels) ^
            SettingsHelper.CreateSetting(document, parent, "LabelsColor", LabelsColor) ^
            SettingsHelper.CreateSetting(document, parent, "IncrementKey", IncrementKey) ^
            SettingsHelper.CreateSetting(document, parent, "DecrementKey", DecrementKey) ^
            SettingsHelper.CreateSetting(document, parent, "ResetKey", ResetKey) ^
            SettingsHelper.CreateSetting(document, parent, "SaveKey", SaveKey);
            SettingsHelper.CreateSetting(document, parent, "ShowProgress", ShowProgress);
            SettingsHelper.CreateSetting(document, parent, "ShowProgressTop", ShowProgressTop);
            SettingsHelper.CreateSetting(document, parent, "ShowProgressText", ShowProgressText);
            SettingsHelper.CreateSetting(document, parent, "ShowProgressBar", ShowProgressBar);
            SettingsHelper.CreateSetting(document, parent, "ShowSplitCount", ShowSplitCount);
            SettingsHelper.CreateSetting(document, parent, "ShowPercentage", ShowPercentage);
            SettingsHelper.CreateSetting(document, parent, "ProgressText", ProgressText);
            SettingsHelper.CreateSetting(document, parent, "ShowProgressCurrentIndex", ShowProgressCurrentIndex);

            XmlElement columnsElement = null;
            if (document != null)
            {
                columnsElement = document.CreateElement("Columns");
                parent.AppendChild(columnsElement);
            }

            var count = 1;
            foreach (var columnData in ColumnsList.Select(x => x.Data))
            {
                XmlElement settings = null;
                if (document != null)
                {
                    settings = document.CreateElement("Settings");
                    columnsElement.AppendChild(settings);
                }
                hashCode ^= columnData.CreateElement(document, settings) * count;
                count++;
            }

            return hashCode;
        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            SettingsHelper.ColorButtonClick((Button)sender, this);
        }

        private void ResetColumns()
        {
            ClearLayout();
            var index = 1;
            foreach (var column in ColumnsList)
            {
                UpdateLayoutForColumn();
                AddColumnToLayout(column, index);
                column.UpdateEnabledButtons();
                index++;
            }
        }

        private void AddColumnToLayout(ColumnSettings column, int index)
        {
            tableColumns.Controls.Add(column, 0, index);
            tableColumns.SetColumnSpan(column, 4);
            column.ColumnRemoved -= column_ColumnRemoved;
            column.MovedUp -= column_MovedUp;
            column.MovedDown -= column_MovedDown;
            column.ColumnRemoved += column_ColumnRemoved;
            column.MovedUp += column_MovedUp;
            column.MovedDown += column_MovedDown;
        }

        void column_MovedDown(object sender, EventArgs e)
        {
            var column = (ColumnSettings)sender;
            var index = ColumnsList.IndexOf(column);
            ColumnsList.Remove(column);
            ColumnsList.Insert(index + 1, column);
            ResetColumns();
            column.SelectControl();
        }

        void column_MovedUp(object sender, EventArgs e)
        {
            var column = (ColumnSettings)sender;
            var index = ColumnsList.IndexOf(column);
            ColumnsList.Remove(column);
            ColumnsList.Insert(index - 1, column);
            ResetColumns();
            column.SelectControl();
        }

        void column_ColumnRemoved(object sender, EventArgs e)
        {
            var column = (ColumnSettings)sender;
            var index = ColumnsList.IndexOf(column);
            ColumnsList.Remove(column);
            ResetColumns();
            if (ColumnsList.Count > 0)
                ColumnsList.Last().SelectControl();
            else
                chkColumnLabels.Select();
        }

        private void ClearLayout()
        {
            tableColumns.RowCount = 1;
            tableColumns.RowStyles.Clear();
            tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, 29f));
            tableColumns.Size = StartingTableLayoutSize;
            foreach (var control in tableColumns.Controls.OfType<ColumnSettings>().ToList())
            {
                tableColumns.Controls.Remove(control);
            }
            Size = StartingSize;
        }

        private void UpdateLayoutForColumn()
        {
            tableColumns.RowCount++;
            tableColumns.RowStyles.Add(new RowStyle(SizeType.Absolute, 179f));
            tableColumns.Size = new Size(tableColumns.Size.Width, tableColumns.Size.Height + 179);
            Size = new Size(Size.Width, Size.Height + 179);
            groupColumns.Size = new Size(groupColumns.Size.Width, groupColumns.Size.Height + 179);
        }

        private void btnAddColumn_Click(object sender, EventArgs e)
        {
            UpdateLayoutForColumn();

            var columnControl = new ColumnSettings(CurrentState, "#" + (ColumnsList.Count + 1), ColumnsList);
            ColumnsList.Add(columnControl);
            AddColumnToLayout(columnControl, ColumnsList.Count);

            foreach (var column in ColumnsList)
                column.UpdateEnabledButtons();
        }

        // Behaviour essentially Lifted from LiveSplit Settings.
        private void SetHotkeyHandlers(TextBox txtBox, Action<KeyOrButton> keySetCallback)
        {
            string oldText = txtBox.Text;
            txtBox.Text = "Set Hotkey...";
            txtBox.Select(0, 0);

            KeyEventHandler handlerDown = null;
            KeyEventHandler handlerUp = null;
            EventHandler leaveHandler = null;
            EventHandlerT<GamepadButton> gamepadButtonPressed = null;

            // Remove Input handlers.
            Action unregisterEvents = () =>
            {
                txtBox.KeyDown -= handlerDown;
                txtBox.KeyUp -= handlerUp;
                txtBox.Leave -= leaveHandler;
                Hook.AnyGamepadButtonPressed -= gamepadButtonPressed;
            };

            // Handler for KeyDown
            handlerDown = (s, x) =>
            {
                KeyOrButton keyOrButton = x.KeyCode == Keys.Escape ? null : new KeyOrButton(x.KeyCode | x.Modifiers);

                // No action for special keys.
                if (x.KeyCode == Keys.ControlKey || x.KeyCode == Keys.ShiftKey || x.KeyCode == Keys.Menu)
                    return;

                keySetCallback(keyOrButton);
                unregisterEvents();

                // Remove Focus.
                txtBox.Select(0, 0);
                //chkGlobalHotKeys.Select();

                txtBox.Text = FormatKey(keyOrButton);

                // Re-Register inputs.
                RegisterHotKeys();
            };

            // Handler for KeyUp (allows setting of special keys, shift, ctrl etc.).
            handlerUp = (s, x) =>
            {
                KeyOrButton keyOrButton = x.KeyCode == Keys.Escape ? null : new KeyOrButton(x.KeyCode | x.Modifiers);

                // No action for normal keys.
                if (x.KeyCode != Keys.ControlKey && x.KeyCode != Keys.ShiftKey && x.KeyCode != Keys.Menu)
                    return;

                keySetCallback(keyOrButton);
                unregisterEvents();
                txtBox.Select(0, 0);
                //chkGlobalHotKeys.Select();
                txtBox.Text = FormatKey(keyOrButton);
                RegisterHotKeys();
            };

            leaveHandler = (s, x) =>
            {
                unregisterEvents();
                txtBox.Text = oldText;
            };

            // Handler for gamepad/joystick inputs.
            gamepadButtonPressed = (s, x) =>
            {
                KeyOrButton key = new KeyOrButton(x);
                keySetCallback(key);
                unregisterEvents();

                Action keyOrButton = () =>
                {
                    txtBox.Select(0, 0);
                    //chkGlobalHotKeys.Select();
                    txtBox.Text = FormatKey(key);
                    RegisterHotKeys();
                };

                // May not be in the UI thread (likely).
                if (InvokeRequired)
                    Invoke(keyOrButton);
                else
                    keyOrButton();
            };

            txtBox.KeyDown += handlerDown;
            txtBox.KeyUp += handlerUp;
            txtBox.Leave += leaveHandler;

            Hook.AnyGamepadButtonPressed += gamepadButtonPressed;
        }

        /// <summary>
        /// Registers the hot keys (unregisters existing Hotkeys).
        /// </summary>
        private void RegisterHotKeys()
        {
            try
            {
                UnregisterAllHotkeys(Hook);

                Hook.RegisterHotKey(IncrementKey);
                Hook.RegisterHotKey(DecrementKey);
                Hook.RegisterHotKey(ResetKey);
                Hook.RegisterHotKey(SaveKey);
            }
            catch (Exception ex)
            {  }
        }

        /// <summary>
        /// Unregisters all hotkeys.
        /// </summary>
        public void UnregisterAllHotkeys(CompositeHook hook)
        {
            hook.UnregisterAllHotkeys();
            HotkeyHook.Instance.UnregisterAllHotkeys();
        }

        private string FormatKey(KeyOrButton key)
        {
            if (key == null)
                return "None";
            string str = key.ToString();
            if (key.IsButton)
            {
                int length = str.LastIndexOf(' ');
                if (length != -1)
                    str = str.Substring(0, length);
            }
            return str;
        }

        private void txtIncrement_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => IncrementKey = x);
        }

        private void txtIncrement_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void txtDecrement_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => DecrementKey = x);
        }

        private void txtDecrement_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void txtReset_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => ResetKey = x);
        }

        private void txtReset_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void txtSave_Enter(object sender, EventArgs e)
        {
            SetHotkeyHandlers((TextBox)sender, x => SaveKey = x);
        }

        private void txtSave_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void rdoTop_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShowProgressTop(null, null);
            ShowProgressTop = 0;
            SplitLayoutChanged(this, null);
        }
        private void rdoMiddle_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShowProgressTop(null, null);
            ShowProgressTop = 1;
            SplitLayoutChanged(this, null);
        }
        private void rdoBottom_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShowProgressTop(null, null);
            ShowProgressTop = 2;
            SplitLayoutChanged(this, null);
        }
        private void rdoShowProgressCurrentIndex_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShowProgressCurrentIndex(null, null);
            ShowProgressCurrentIndex = 0;
            SplitLayoutChanged(this, null);
        }
        private void rdoShowProgressNextIndex_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShowProgressCurrentIndex(null, null);
            ShowProgressCurrentIndex = 1;
            SplitLayoutChanged(this, null);
        }


        private void chkProgressBar_CheckedChanged(object sender, EventArgs e)
        {
            ShowProgressBar = chkProgressBar.Checked;
            SplitLayoutChanged(this, null);
        }

        private void chkSplitCount_CheckedChanged(object sender, EventArgs e)
        {
            ShowSplitCount = chkSplitCount.Checked;
            SplitLayoutChanged(this, null);
        }

        private void chkPercentage_CheckedChanged(object sender, EventArgs e)
        {
            ShowPercentage = chkPercentage.Checked;
            SplitLayoutChanged(this, null);
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            ProgressText = txtText.Text;
        }

        private void groupBoxShowProgress_Enter(object sender, EventArgs e)
        {

        }
    }
}
