using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Forms
{
    internal static class EventCardFactory
    {
        private static readonly Color Border = Color.FromArgb(210, 214, 220);
        private static readonly Color Muted = Color.FromArgb(90, 98, 110);
        private static readonly Color Title = Color.FromArgb(17, 24, 39);
        private static readonly Font DateFont = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
        private static readonly Font ReasonFont = new Font("Segoe UI", 8.5F, FontStyle.Italic);
        private static readonly Font MetaFont = new Font("Segoe UI", 8.5F);
        private static readonly Font CompactMetaFont = new Font("Segoe UI", 8F);

        public static Panel CreateEventCard(LocalEvent item, int width, Action<LocalEvent> onOpen)
        {
            var card = CreateCard(item, width, 138, AccentFor(item));
            int textWidth = width - 28;

            card.Controls.Add(MakeLabel(item.StartDate.ToString("ddd dd MMM yyyy"), 14, 8, textWidth, 18, UITheme.Navy, DateFont));
            card.Controls.Add(MakeLabel(item.KindLabel + "  ·  " + item.Category, 14, 28, textWidth, 18, Muted, UITheme.BodyFont));
            card.Controls.Add(MakeLabel(item.Title, 14, 50, textWidth, 24, Title, UITheme.LabelFont));
            card.Controls.Add(MakeLabel(item.Location, 14, 76, textWidth, 18, Muted, UITheme.BodyFont));
            card.Controls.Add(MakeLabel(Shorten(item.Description, 110), 14, 98, textWidth, 32, Title, UITheme.BodyFont));
            Wire(card, item, onOpen);
            return card;
        }

        public static Panel CreatePriorityCard(LocalEvent item, int order, Action<LocalEvent> onOpen)
        {
            const int width = 250;
            var card = CreateCard(item, width, 96, AccentFor(item));
            card.Margin = new Padding(0, 0, 10, 0);

            card.Controls.Add(MakeLabel(order + ".  " + item.PriorityLabel, 14, 8, width - 28, 18, AccentFor(item), DateFont));
            card.Controls.Add(MakeLabel(item.Title, 14, 28, width - 28, 22, Title, UITheme.LabelFont));
            card.Controls.Add(MakeLabel(item.StartDate.ToString("ddd dd MMM") + "  ·  " + item.Location, 14, 54, width - 28, 32, Muted, UITheme.BodyFont));
            Wire(card, item, onOpen);
            return card;
        }

        public static Panel CreateRecommendationCard(RecommendedEvent recommendation, int width, Action<LocalEvent> onOpen)
        {
            LocalEvent item = recommendation.Event;
            var card = CreateCard(item, width, 78, UITheme.Gold);
            int textWidth = width - 28;

            card.Controls.Add(MakeLabel(item.Title, 14, 8, textWidth, 20, Title, UITheme.LabelFont));
            card.Controls.Add(MakeLabel(recommendation.Reason, 14, 30, textWidth, 18, UITheme.Navy, ReasonFont));
            card.Controls.Add(MakeLabel(item.StartDate.ToString("dd MMM yyyy") + "  ·  " + item.Category, 14, 50, textWidth, 18, Muted, MetaFont));
            Wire(card, item, onOpen);
            return card;
        }

        public static Panel CreateCompactRow(LocalEvent item, int width, string leading, Action<LocalEvent> onOpen)
        {
            var card = CreateCard(item, width, 40, UITheme.Navy);
            card.Controls.Add(MakeLabel(leading + item.Title, 12, 4, width - 24, 16, Title, UITheme.LabelFont));
            card.Controls.Add(MakeLabel(item.StartDate.ToString("dd MMM") + "  ·  " + item.Category, 12, 20, width - 24, 16, Muted, CompactMetaFont));
            Wire(card, item, onOpen);
            return card;
        }

        private static Panel CreateCard(LocalEvent item, int width, int height, Color accent)
        {
            var card = new Panel
            {
                Width = width,
                Height = height,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 12, 12),
                Cursor = Cursors.Hand,
                Tag = item
            };

            Color accentColor = accent;
            card.Paint += (sender, args) =>
            {
                using (var borderPen = new Pen(Border))
                {
                    args.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
                }

                using (var accentBrush = new SolidBrush(accentColor))
                {
                    args.Graphics.FillRectangle(accentBrush, 0, 0, 5, card.Height);
                }
            };

            return card;
        }

        private static Label MakeLabel(string text, int x, int y, int width, int height, Color color, Font font)
        {
            return new Label
            {
                Text = text ?? string.Empty,
                Location = new Point(x, y),
                Size = new Size(width, height),
                ForeColor = color,
                Font = font,
                BackColor = Color.White,
                AutoEllipsis = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
        }

        private static void Wire(Control card, LocalEvent item, Action<LocalEvent> onOpen)
        {
            EventHandler handler = (sender, args) => onOpen(item);
            card.Click += handler;
            foreach (Control child in card.Controls)
            {
                child.Click += handler;
            }
        }

        private static Color AccentFor(LocalEvent item)
        {
            if (item.IsAnnouncement && item.Priority >= LocalEvent.PriorityUrgent)
            {
                return UITheme.Danger;
            }

            if (item.IsAnnouncement)
            {
                return UITheme.Gold;
            }

            return UITheme.Navy;
        }

        private static string Shorten(string value, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= maxLength)
            {
                return value ?? string.Empty;
            }

            return value.Substring(0, maxLength - 1).TrimEnd() + "…";
        }
    }
}
