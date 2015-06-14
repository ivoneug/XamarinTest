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
	public class ViewPagerAdapter : FragmentPagerAdapter, ViewPager.IOnPageChangeListener
	{
		private MainActivity _context;
		private FragmentManager _fragmentManager;

		private int _lastSelectedPage = 0;

		private ViewPagerItem[] _itemsArray;

		public ViewPagerAdapter(MainActivity context, FragmentManager fragmentManager, ViewPagerItem[] itemsArray) : base(fragmentManager) {
			_fragmentManager = fragmentManager;
			_context = context;
			_itemsArray = itemsArray;
		}

		public override int Count {
			get {
				return _itemsArray.Length * MainActivity.LoopsCount;
			}
		}

		public override Fragment GetItem(int position){
			position = CalcActualItemPosition (position);
			ViewPagerItemFragment fragment = (ViewPagerItemFragment)ViewPagerItemFragment.NewInstance (_context, position, _itemsArray[position]);

			return fragment;
		}

		public void OnPageScrolled(int position, float positionOffset, int positionOffsetPixels)
		{
		}

		public void OnPageSelected(int position) {
			ViewPagerItemFragment currentPage = GetPageFragment(position);
			ViewPagerItemFragment previousPage = GetPageFragment(_lastSelectedPage);

			if (currentPage != null) {
				currentPage.SetPageTextColor (true);
				if (currentPage.BoundedItem != null) {
					currentPage.BoundedItem.IsSelected = true;
				}
			}
			if (previousPage != null && position != _lastSelectedPage) {
				previousPage.SetPageTextColor (false);
				if (previousPage.BoundedItem != null) {
					previousPage.BoundedItem.IsSelected = false;
				}
			}

			//очищаем выбранные элементы в старом расписании
			ClearBookingInTimetable (_lastSelectedPage);

			_lastSelectedPage = position;

			//обновляем список новым расписанием
			UpdateTimetable (position);
		}
	
		public void OnPageScrollStateChanged(int state) {
		}

		/// <summary>
		/// Вычисляет реальный номре страницы
		/// </summary>
		/// <returns>Реальный номер позиции.</returns>
		/// <param name="position">Исходный номер позиции.</param>
		private int CalcActualItemPosition(int position){
			return position % _itemsArray.Length;
		}

		/// <summary>
		/// Перейти к следующему элементу
		/// </summary>
		public void NextItem(){
			_context.Pager.SetCurrentItem(_lastSelectedPage + 1, true);
		}

		/// <summary>
		/// Вернуться к предыдущему элементу
		/// </summary>
		public void PrevItem(){
			_context.Pager.SetCurrentItem(_lastSelectedPage - 1, true);
		}

		/// <summary>
		/// Обновить связанное расписание
		/// </summary>
		/// <param name="position">позиция.</param>
		private void UpdateTimetable(int position){
			if (_context.TimetableAdapter == null) {
				return;
			}

			position = CalcActualItemPosition (position);

			ViewPagerItem item = _itemsArray [position];
			_context.TimetableAdapter.Clear ();
			_context.TimetableAdapter.AddAll (item.CurrentTimetable);

			//за одно запрещаем кнопку букинга
			_context.BookButton.Enabled = false;
		}

		/// <summary>
		/// Обновить связанное расписание
		/// </summary>
		public void UpdateTimetable(){
			UpdateTimetable (_lastSelectedPage);
		}

		private void ClearBookingInTimetable(int position){
			position = CalcActualItemPosition (position);
			ViewPagerItem item = _itemsArray [position];

			for(int idx = 0; idx < item.CurrentTimetable.Length; idx++){
				TimetableItem otherItem = item.CurrentTimetable [idx];
				otherItem.Booked = false;
			}

			//за одно запрещаем кнопку букинга
			_context.BookButton.Enabled = false;
		}

		/// <summary>
		/// Возвращает фрагмент страницы по позиции (ищет в теге)
		/// </summary>
		/// <returns>The page fragment.</returns>
		/// <param name="position">Position.</param>
		private ViewPagerItemFragment GetPageFragment(int position){
			return (ViewPagerItemFragment)_fragmentManager.FindFragmentByTag (GetFragmentTag (position));
		}

		/// <summary>
		/// Формирует тег для поиска фрагмента
		/// </summary>
		/// <returns>The fragment tag.</returns>
		/// <param name="position">Position.</param>
		private String GetFragmentTag(int position)
		{
			return "android:switcher:" + _context.Pager.Id + ":" + position;
		}
	}
}

