using System.Drawing;

namespace MunicipalServicesApp
{
    public static class UITheme
    {
        public static readonly Color Navy = Color.FromArgb(26, 54, 93);
        public static readonly Color NavyDark = Color.FromArgb(18, 38, 68);
        public static readonly Color Gold = Color.FromArgb(240, 180, 60);
        public static readonly Color Background = Color.WhiteSmoke;
        public static readonly Color Panel = Color.White;
        public static readonly Color Text = Color.FromArgb(33, 37, 41);
        public static readonly Color Muted = Color.FromArgb(105, 105, 105);
        public static readonly Color Success = Color.FromArgb(26, 120, 60);
        public static readonly Color Danger = Color.FromArgb(176, 42, 55);
        public static readonly Color Disabled = Color.FromArgb(180, 186, 194);
        public static readonly Font HeaderFont = new Font("Segoe UI", 20F, FontStyle.Bold);
        public static readonly Font SubHeaderFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font TitleFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        public static readonly Font BodyFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        public static readonly Font ButtonFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font LabelFont = new Font("Segoe UI", 10F, FontStyle.Bold);
    }
}
