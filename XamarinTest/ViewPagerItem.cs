using System;
using System.Collections.Generic;
using System.Globalization;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace XamarinTest
{
	public class ViewPagerItem
	{
		private CultureInfo info = new CultureInfo ("ru-RU");

		/// <summary>
		/// Текущая дата
		/// </summary>
		public DateTime CurrentDate { get; private set;}

		/// <summary>
		/// Наименование текущей даты (двухзначное число)
		/// </summary>
		public string DateName{
			get{
				return CurrentDate.Date.ToString ("dd");
			}
		}

		/// <summary>
		/// Краткое наименование дня недели
		/// </summary>
		public string DayOfWeekName{
			get{
				return CurrentDate.ToString ("ddd", info).ToLower();
			}
		}

		/// <summary>
		/// Возвращает значение, определяющее что текущий <see cref="XamarinTest.ViewPagerItem"/> доступен.
		/// </summary>
		/// <value><c>true</c> если доступен; иначе, <c>false</c>.</value>
		public bool Avability{ get; private set;}

		/// <summary>
		/// Возвращает значение, определяющее что текущий элемент описывает выходной
		/// </summary>
		/// <value><c>true</c> если выходной; иначе, <c>false</c>.</value>
		public bool IsDayOff{ get; private set;}

		/// <summary>
		/// Является ли день сегодняшним днем
		/// </summary>
		/// <value><c>true</c> если это сегодняшний день; иначе, <c>false</c>.</value>
		public bool IsToday{ get; private set;}

		/// <summary>
		/// Выбран ли элемент в данный момент
		/// </summary>
		/// <value><c>true</c> if this instance is selected; otherwise, <c>false</c>.</value>
		public bool IsSelected { get; set;}

		private List<TimetableItem> _currentDateTimetable = new List<TimetableItem> ();
		public TimetableItem[] CurrentTimetable{get{ return _currentDateTimetable.ToArray (); }}

		private Context _context;
		private int[] _stringIds;

		public ViewPagerItem (Context context, DateTime currentDate, bool isAvailable)
		{
			_context = context;

			CurrentDate = currentDate.Date;
			IsDayOff = (CurrentDate.DayOfWeek == DayOfWeek.Saturday || CurrentDate.DayOfWeek == DayOfWeek.Sunday);
			Avability = isAvailable;

			IsToday = CurrentDate == DateTime.Now.Date;
			//при создании - сегодняшний элемент всегда выбран
			IsSelected = IsToday;

			GenerateRandomTimetable ();
		}

		/// <summary>
		/// Генерирует случайное расписание
		/// </summary>
		private void GenerateRandomTimetable(){
			_stringIds = new int[10];
			_stringIds[0] = Resource.String.timetable_descr1;
			_stringIds[1] = Resource.String.timetable_descr2;
			_stringIds[2] = Resource.String.timetable_descr3;
			_stringIds[3] = Resource.String.timetable_descr4;
			_stringIds[4] = Resource.String.timetable_descr5;
			_stringIds[5] = Resource.String.timetable_descr6;
			_stringIds[6] = Resource.String.timetable_descr7;
			_stringIds[7] = Resource.String.timetable_descr8;
			_stringIds[8] = Resource.String.timetable_descr9;
			_stringIds[9] = Resource.String.timetable_descr10;

			_currentDateTimetable.Clear ();

			Random random = new Random (System.Environment.TickCount);
			DateTime dt = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);

			for (int idx = 0; idx < 10; idx++) {
				
				bool isAvailable = Avability ? random.Next (0, 10) > 5 : false;

				//выбираем случайную строку из списка строк для расписания
				int stringId = 0;
				while (true) {
					int sIdx = random.Next (0, 10);
					stringId = _stringIds [sIdx];
					if (stringId != 0) {
						_stringIds [sIdx] = 0;
						break;
					}
				}

				TimetableItem item = new TimetableItem (this, dt.ToString ("HH:mm"), _context.Resources.GetString(stringId), isAvailable);
				_currentDateTimetable.Add (item);

				dt = dt.AddHours (1).AddMinutes (30);
			}
		}
	}
}

