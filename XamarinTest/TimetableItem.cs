using System;

namespace XamarinTest
{
	public class TimetableItem
	{
		/// <summary>
		/// Текст времени (в формате 00:00)
		/// </summary>
		/// <value>The time.</value>
		public String Time{ get; private set;}

		/// <summary>
		/// Описание
		/// </summary>
		/// <value>The description.</value>
		public String Description{ get; private set;}

		/// <summary>
		/// Возвращает значение, определяющее что текущий <see cref="XamarinTest.TimetableItem"/> доступен
		/// </summary>
		/// <value><c>true</c> если доступен; иначе, <c>false</c>.</value>
		public bool Avability{ get; private set;}

		private bool _booked = false;
		/// <summary>
		/// Возвращает значение, определяющее что текущий <see cref="XamarinTest.TimetableItem"/> забронирован.
		/// </summary>
		/// <value><c>true</c> если забронирован; иначе, <c>false</c>.</value>
		public bool Booked {
			get{ return _booked; }
			set{
				if(!Avability) return;
				_booked = value;
			}
		}

		/// <summary>
		/// Основной элемент расписания
		/// </summary>
		/// <value>The today item.</value>
		public ViewPagerItem TodayItem{ get; private set;}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="XamarinTest.TimetableItem"/>
		/// </summary>
		/// <param name="time">Текст времени</param>
		/// <param name="description">Описание.</param>
		public TimetableItem(ViewPagerItem todayItem, String time, String description){
			TodayItem = todayItem;
			Time = time;
			Description = description;
			Avability = true;
		}

		/// <summary>
		/// Инициализирует новый экземпляр класса <see cref="XamarinTest.TimetableItem"/>
		/// </summary>
		/// <param name="time">Текст времени</param>
		/// <param name="description">Описание</param>
		/// <param name="avability">Установить в <c>true</c> если доступен для бронирования</param>
		public TimetableItem(ViewPagerItem todayItem, String time, String description, bool avability) : this(todayItem, time, description){
			Avability = avability;
		}
	}
}

