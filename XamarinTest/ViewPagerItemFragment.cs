using System;

using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace XamarinTest
{
	public class ViewPagerItemFragment : Fragment
	{
		/// <summary>
		/// ViewHolder для страницы, привет производительность!
		/// </summary>
		public class ViewPagerItemHolder : Java.Lang.Object{
			public TextView DayNumberText { get; private set;}
			public TextView DayOfWeekText{ get; private set;}

			public ViewPagerItemHolder(View view){
				DayNumberText = view.FindViewById<TextView>(Resource.Id.date_value_text);
				DayOfWeekText = view.FindViewById<TextView>(Resource.Id.date_name_text);
			}
		}

		/// <summary>
		/// Связанный элемент с данными для нашего фрагмента
		/// </summary>
		/// <value>The bounded item.</value>
		public ViewPagerItem BoundedItem{ get; set;}
		private LinearLayout _rootLayout = null;

		/// <summary>
		/// Фабрика для создания и инициализации наших фрагментов
		/// </summary>
		/// <returns>The instance.</returns>
		/// <param name="context">Context.</param>
		/// <param name="pos">Position.</param>
		/// <param name="item">Item.</param>
		public static Fragment NewInstance(MainActivity context, int position, ViewPagerItem item)
		{
			Bundle bundle = new Bundle();
			//нужно, что бы если Андроид убьет фрагмент, корректно привязать к нему элемент из датасурса
			bundle.PutInt ("position", position);

			ViewPagerItemFragment fragment = (ViewPagerItemFragment)Fragment.Instantiate(context, (new ViewPagerItemFragment()).Class.Name, bundle);

			return fragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
			if (container == null) {
				return null;
			}

			LinearLayout view = (LinearLayout)inflater.Inflate(Resource.Layout.view_pager_item_fragment, container, false);
			_rootLayout = view;
			ViewPagerItemHolder holder = new ViewPagerItemHolder(view);
			view.Tag = holder;

			//привязываем элемент из датасурса
			if (BoundedItem == null) {
				BoundedItem = ((MainActivity)Activity).Model.DataSourceItems [Arguments.GetInt ("position")];
			}

			if (BoundedItem != null) {
				holder.DayNumberText.Text = BoundedItem.DateName;
				holder.DayOfWeekText.Text = BoundedItem.DayOfWeekName;

				SetPageTextColor (BoundedItem.IsSelected);
			}

			return view;
		}

		/// <summary>
		/// Устанавливает цвет для страницы в соответствии с ее активностью и доступностью
		/// </summary>
		/// <param name="isActivePage">Если <c>true</c> то считать активной страницей.</param>
		public void SetPageTextColor(bool isActivePage){
			if (BoundedItem == null) {
				return;
			}

			if (isActivePage) {
				if (BoundedItem.Avability) {
					if (BoundedItem.IsDayOff) {
						SetPageTextColor (Resource.Color.view_pager_item_dayoff, true);
					} else {
						SetPageTextColor (Resource.Color.view_pager_item_accent, true);
					}
				} else {
					SetPageTextColor (Resource.Color.view_pager_item_disabled, true);
				}
			} else {
				if (BoundedItem.Avability) {
					if (BoundedItem.IsDayOff) {
						SetPageTextColor (Resource.Color.view_pager_item_dayoff, false);
					} else {
						SetPageTextColor (Resource.Color.view_pager_item_color, false);
					}
				} else {
					SetPageTextColor (Resource.Color.view_pager_item_disabled, false);
				}
			}
		}

		private void SetPageTextColor(int colorId, bool bold){
			if (_rootLayout == null) {
				return;
			}

			ViewPagerItemFragment.ViewPagerItemHolder handler = (ViewPagerItemFragment.ViewPagerItemHolder)_rootLayout.Tag;
			handler.DayNumberText.SetTextColor(Activity.Resources.GetColor(colorId));
			handler.DayOfWeekText.SetTextColor(Activity.Resources.GetColor(colorId));

			if (bold) {
				handler.DayNumberText.SetTypeface (handler.DayNumberText.Typeface, Android.Graphics.TypefaceStyle.Bold);
				handler.DayOfWeekText.SetTypeface (handler.DayOfWeekText.Typeface, Android.Graphics.TypefaceStyle.Bold);
			} else {
				handler.DayNumberText.SetTypeface (null, Android.Graphics.TypefaceStyle.Normal);
				handler.DayOfWeekText.SetTypeface (null, Android.Graphics.TypefaceStyle.Normal);
			}
		}
	}
}

