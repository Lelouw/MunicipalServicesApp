using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public static class StatusBadge
    {
        public static void Draw(Graphics graphics, Rectangle cellBounds, string status, Font font)
        {
            if (graphics == null || font == null)
            {
                return;
            }

            string text = string.IsNullOrWhiteSpace(status) ? "Pending" : status;
            Color backColor;
            Color foreColor;
            Colors(text, out backColor, out foreColor);

            Size textSize = TextRenderer.MeasureText(text, font);
            int badgeWidth = Math.Min(cellBounds.Width - 12, Math.Max(84, textSize.Width + 18));
            int badgeHeight = 22;
            var badgeRect = new Rectangle(
                cellBounds.X + 8,
                cellBounds.Y + Math.Max(2, (cellBounds.Height - badgeHeight) / 2),
                Math.Max(20, badgeWidth),
                badgeHeight);

            using (GraphicsPath path = RoundedRect(badgeRect, badgeHeight / 2))
            using (var brush = new SolidBrush(backColor))
            {
                graphics.FillPath(brush, path);
            }

            TextRenderer.DrawText(
                graphics,
                text,
                font,
                badgeRect,
                foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        public static void Colors(string status, out Color backColor, out Color foreColor)
        {
            switch (status)
            {
                case "Acknowledged":
                    backColor = Color.FromArgb(237, 233, 254);
                    foreColor = Color.FromArgb(91, 33, 182);
                    break;
                case "Assigned":
                    backColor = Color.FromArgb(204, 251, 241);
                    foreColor = Color.FromArgb(17, 94, 89);
                    break;
                case "In Progress":
                    backColor = Color.FromArgb(220, 252, 231);
                    foreColor = Color.FromArgb(22, 101, 52);
                    break;
                case "Resolved":
                    backColor = Color.FromArgb(219, 234, 254);
                    foreColor = Color.FromArgb(30, 64, 175);
                    break;
                case "Closed":
                    backColor = Color.FromArgb(229, 231, 235);
                    foreColor = Color.FromArgb(75, 85, 99);
                    break;
                default:
                    backColor = Color.FromArgb(254, 243, 199);
                    foreColor = Color.FromArgb(146, 64, 14);
                    break;
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = Math.Max(2, radius * 2);
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
