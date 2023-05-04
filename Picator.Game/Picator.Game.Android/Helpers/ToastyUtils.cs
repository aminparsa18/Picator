using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using AndroidX.AppCompat.Content.Res;
using AndroidX.Core.Content;

namespace Picator.Game.Droid.Helpers
{
    public class ToastyUtils
    {
        public static Drawable TintIcon(Drawable drawable, Color tintColor)
        {
            drawable.SetColorFilter(new PorterDuffColorFilter(tintColor, PorterDuff.Mode.SrcIn));
            return drawable;
        }

        public static Drawable Tint9PatchDrawableFrame(Context context, Color tintColor)
        {
            var toastDrawable = (NinePatchDrawable)GetDrawable(context, Resource.Drawable.toast_frame);
            return TintIcon(toastDrawable, tintColor);
        }

        public static void SetBackground(Android.Views.View view, Drawable drawable)
        {
            view.Background = drawable;
        }

        public static Drawable GetDrawable(Context context, int id)
        {
            return AppCompatResources.GetDrawable(context, id);
        }

        public static Color GetColor(Context context, int color)
        {
            return new Color(ContextCompat.GetColor(context, color));
        }
    }
}