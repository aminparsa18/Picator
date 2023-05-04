using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Picator.Game.Droid.Helpers
{
    public class Toasty
    {
        private const bool TintIcon = true;
        private const int TextSize = 16; // in SP

        public static Toast Normal(Context context, string message)
        {
            return Custom(context, message, null,
                ToastyUtils.GetColor(context, Resource.Color.normalColor),
                ToastyUtils.GetColor(context, Resource.Color.defaultTextColor),
                ToastLength.Short, false, false);
        }

        public static Toast Success(Context context, string message)
        {
            return Custom(context, message, ToastyUtils.GetDrawable(context, Resource.Drawable.ic_check_white_24dp),
                ToastyUtils.GetColor(context, Resource.Color.successColor),
                ToastyUtils.GetColor(context, Resource.Color.defaultTextColor),
                ToastLength.Short, true, true);
        }

        public static Toast Error(Context context, string message)
        {
            return Custom(context, message, ToastyUtils.GetDrawable(context, Resource.Drawable.ic_clear_white_24dp),
                ToastyUtils.GetColor(context, Resource.Color.errorColor),
                ToastyUtils.GetColor(context, Resource.Color.defaultTextColor),
                ToastLength.Short, true, true);
        }

        public static Toast Warning(Context context, string message)
        {
            return Custom(context, message,
                ToastyUtils.GetDrawable(context, Resource.Drawable.ic_error_outline_white_24dp),
                ToastyUtils.GetColor(context, Resource.Color.warningColor),
                ToastyUtils.GetColor(context, Resource.Color.defaultTextColor),
                ToastLength.Short, true, true);
        }

        public static Toast Info(Context context, string message)
        {
            return Custom(context, message,
                ToastyUtils.GetDrawable(context, Resource.Drawable.ic_info_outline_white_24dp),
                ToastyUtils.GetColor(context, Resource.Color.infoColor),
                ToastyUtils.GetColor(context, Resource.Color.defaultTextColor),
                ToastLength.Short, true, true);
        }

        private static Toast Custom(Context context, string message, Drawable icon,
            Color tintColor, Color textColor, ToastLength duration,
            bool withIcon, bool shouldTint)
        {
            var currentToast = Toast.MakeText(context, "", duration);
            var toastLayout = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService))
                .Inflate(Resource.Layout.toast_layout, null);
            var toastIcon = toastLayout.FindViewById<ImageView>(Resource.Id.toast_icon);
            var toastTextView = toastLayout.FindViewById<TextView>(Resource.Id.toast_text);

            var drawableFrame = shouldTint
                ? ToastyUtils.Tint9PatchDrawableFrame(context, tintColor)
                : ToastyUtils.GetDrawable(context, Resource.Drawable.toast_frame);
            ToastyUtils.SetBackground(toastLayout, drawableFrame);

            if (withIcon)
            {
                if (icon == null)
                    throw new IllegalArgumentException("Avoid passing 'icon' as null if 'withIcon' is set to true");
                ToastyUtils.SetBackground(toastIcon, TintIcon ? ToastyUtils.TintIcon(icon, textColor) : icon);
            }
            else
            {
                toastIcon.Visibility = ViewStates.Gone;
            }

            toastTextView.Text = message;
            toastTextView.SetTextColor(textColor);
            toastTextView.SetTextSize(ComplexUnitType.Sp, TextSize);

            currentToast.View = toastLayout;

            return currentToast;
        }
    }
}