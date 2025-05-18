using LiveSplit.Model;

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LiveSplit.UI.Components
{
    public class ProgressComponent : IComponent
    {
        public SplitsSettings Settings { get; set; }
        public IList<SplitComponent> SplitList { get; set; }
        public GraphicsCache Cache { get; set; }

        public float PaddingTop => 0f;
        public float PaddingLeft => 0f;
        public float PaddingBottom => 0f;
        public float PaddingRight => 0f;

        public float VerticalHeight => 25 + Settings.SplitHeight;

        public float MinimumWidth { get; set; }
        public float HorizontalWidth => 10f;
        public float MinimumHeight { get; set; } = 31;
        protected SimpleLabel TextLabel { get; set; }
        protected SimpleLabel ProgressLabel { get; set; }

        public IDictionary<string, Action> ContextMenuControls => null;

        public ProgressComponent(SplitsSettings settings, IList<SplitComponent> SplitComponents)
        {
            Settings = settings;
            SplitList = SplitComponents;
            if (Settings.ShowProgressText)
            {
                TextLabel = new SimpleLabel()
                {
                    HorizontalAlignment = StringAlignment.Near,
                    X = 8,
                };
            };
            if (Settings.ShowSplitCount || Settings.ShowPercentage)
            {
                ProgressLabel = new SimpleLabel()
                {
                    HorizontalAlignment = StringAlignment.Far,
                    X = 8,
                };
            }
            ;
            MinimumHeight = 25f;
            MinimumHeight = Settings.SplitHeight + 25;
            MinimumWidth = 10f;
            Cache = new GraphicsCache();
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            if (!Settings.ShowProgress)
                return;

            var textSettings = state.LayoutSettings;
            var text = Settings.ProgressText;

            int total;
            if (Settings.AlwaysShowLastSplit)
                total = state.Run.Count - 1; // Exclude the last split, used for counter totals
            else
                total = state.Run.Count;

            int cur = Math.Max(Math.Min(state.CurrentSplitIndex, total), 0); // Ensure cur is within bounds (0 to total)
            int percent = (int)Math.Round((double)cur / total * 100);
            string fraction = $"{cur}/{total}";
            string percentText = $"{percent}%";
            string combined = $"{fraction} ({percentText})";
            string chosenText;

            float barX = 10;
            float barWidth = (int)width - 30;

            float barY;
            float barHeight;
            float fontHeight = state.LayoutSettings.TextFont.Height;
            if (fontHeight > 10)
            {
                barY = 5;
                barHeight = fontHeight - 10;
            }
            else
            {
                barY = (int)Math.Round(fontHeight/2);
                barHeight = fontHeight - 2*barY;
            }

            if (Settings.ShowProgressText && !string.IsNullOrEmpty(text))
            {
                TextLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                TextLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
                TextLabel.VerticalAlignment = StringAlignment.Near;
                TextLabel.Y = 0;
                TextLabel.Height = 50;

                TextLabel.Text = text;
                TextLabel.ForeColor = state.LayoutSettings.TextColor;

                TextLabel.Font = state.LayoutSettings.TextFont;
                TextLabel.X = 5;
                TextLabel.HasShadow = state.LayoutSettings.DropShadows;

                var nameX = width - 7;
                TextLabel.Width = nameX;
                TextLabel.Draw(g);

                barX += g.MeasureString(TextLabel.Text, TextLabel.Font).Width;
                barWidth -= g.MeasureString(TextLabel.Text, TextLabel.Font).Width;
            }
            if (Settings.ShowSplitCount || Settings.ShowPercentage)
            {
                if (!Settings.ShowPercentage)
                {
                    chosenText = fraction;
                }
                else if (!Settings.ShowSplitCount)
                {
                    chosenText = percentText;
                }
                else
                {
                    chosenText = combined;
                }
                ProgressLabel.ShadowColor = state.LayoutSettings.ShadowsColor;
                ProgressLabel.OutlineColor = state.LayoutSettings.TextOutlineColor;
                ProgressLabel.VerticalAlignment = StringAlignment.Near;
                ProgressLabel.Y = 0;
                ProgressLabel.Height = 50;

                ProgressLabel.Text = chosenText;
                ProgressLabel.ForeColor = state.LayoutSettings.TextColor;

                ProgressLabel.Font = state.LayoutSettings.TextFont;
                ProgressLabel.X = 5;
                ProgressLabel.HasShadow = state.LayoutSettings.DropShadows;

                var nameX = width - 7;
                ProgressLabel.Width = nameX;
                ProgressLabel.Draw(g);

                barWidth -= g.MeasureString(ProgressLabel.Text, ProgressLabel.Font).Width;
            }
            if (Settings.ShowProgressBar)
            {
                float segmentWidth = barWidth / total;
                int start = 0;

                while (start < total)
                {
                    // Get the current segment color
                    Color currentColor = GetColorForSplit(start, state, SplitList);
                    int end = start + 1;

                    // Group the next segments with the same color
                    while (end < total && GetColorForSplit(end, state, SplitList) == currentColor)
                        end++;

                    // Calculate start and end to draw the segment
                    float segmentX = barX + start * segmentWidth;
                    float segmentW = (end - start) * segmentWidth;

                    // Draw the segment
                    using (var backBrush = new SolidBrush(currentColor))
                    {
                        g.FillRectangle(backBrush, segmentX, barY, segmentW, barHeight);
                    }

                    start = end;
                }
            }
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
        }

        private void DrawEmpty(Graphics g, float width, float height)
        {
        }
        Color GetColorForSplit(int i, LiveSplitState state, IList<SplitComponent> SplitList)
        {
            SplitComponent split;
            try
            {
                split = SplitList[0];
            }
            catch (ArgumentOutOfRangeException)
            {
                return Settings.BeforeNamesColor;
            }
            int counter = split.CounterList[i];
            int pbCounter = split.GetPBCounterPublic(i);
            if (i < state.CurrentSplitIndex)
            {
                if (!Settings.OverrideTextColor)
                    return state.LayoutSettings.TextColor;
                else if (counter == 0)
                    return Settings.BeforeNamesColor;
                else if (counter < pbCounter)
                    return Settings.BeforeNamesColorLowCounter;
                else if (counter == pbCounter)
                    return Settings.BeforeNamesColorSameCounter;
                else // counter > pbCounter
                    return Settings.BeforeNamesColorHighCounter;
            }
            else if (i == state.CurrentSplitIndex)
            {
                if (!Settings.OverrideTextColor)
                    return state.LayoutSettings.TextColor;
                else if (counter == 0)
                    return Settings.CurrentNamesColor;
                else if (counter < pbCounter)
                    return Settings.CurrentNamesColorLowCounter;
                else if (counter == pbCounter)
                    return Settings.CurrentNamesColorSameCounter;
                else // counter > pbCounter
                    return Settings.CurrentNamesColorHighCounter;
            }
            else
            {
                if (!Settings.OverrideTextColor)
                    return state.LayoutSettings.TextColor;
                else
                    return Settings.AfterNamesColor;
            }
        }

        public string ComponentName => "Progress";

        public Control GetSettingsControl(LayoutMode mode)
        {
            throw new NotSupportedException();
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            throw new NotSupportedException();
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            throw new NotSupportedException();
        }

        public string UpdateName => throw new NotSupportedException();
        public string XMLURL => throw new NotSupportedException();
        public string UpdateURL => throw new NotSupportedException();
        public Version Version => throw new NotSupportedException();

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // Por ahora no hay lógica de actualización
        }
        public void Dispose()
        {
        }
    }
}
