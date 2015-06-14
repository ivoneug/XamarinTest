using System;

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
	[Activity (Label = "XamarinTest", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : FragmentActivity {

		/// <summary>
		/// Фрагмент без интерфейса, будет хранить нашу модель
		/// (данный фрагмент будет ретейниться после уничтожения основной вьюхи и наши данные не будут перегенериваться при пересоздании вьюхи)
		/// По хорошему с текущими данными хорошо бы использовать SQLite и ContentProvider, но время на реализацию ограничено
		/// так что используем такой принцип
		/// </summary>
		public class ModelHolderFragment : Android.App.Fragment{
			public static string SearchTag = "MODEL_HOLDER_FRAGMENT";

			public ViewPagerItem[] DataSourceItems { get; private set;}

			public override void OnCreate (Bundle savedInstanceState)
			{
				base.OnCreate (savedInstanceState);

				DataSourceItems = GenViewPagerItems ();

				//установим флаг, который запретит фрагменту умирать при уничтожении основной вьюхи
				RetainInstance = true;
			}

			/// <summary>
			/// Генерация фейковых данных
			/// </summary>
			/// <returns>The view pager items.</returns>
			private ViewPagerItem[] GenViewPagerItems(){
				Random random = new Random (System.Environment.TickCount);
				DateTime dt = new DateTime (DateTime.Now.Year, DateTime.Now.Month, 1);
				int daysCount = DateTime.DaysInMonth (dt.Year, dt.Month);

				//настраиваем количество страниц
				PagesCount = daysCount;
				int middle = daysCount / 2;			//середина месяца
				int currentDay = DateTime.Now.Day;	//текущий день
				FirstPageNumber = PagesCount * LoopsCount / 2 - middle - 1;	//середина диапазона страниц, от нее отнимаем середину месяца (выравниваем к массиву) и отнимаем 1 день (нумерация в массиве с 0)

				//устанавливаем стартовую дату в текущий день
				if (currentDay < middle) {
					FirstPageNumber -= (middle - currentDay);
				} else if (currentDay > middle) {
					FirstPageNumber += currentDay - middle;
				}

				//генерим дни
				ViewPagerItem[] items = new ViewPagerItem[daysCount];
				for (int idx = 0; idx < daysCount; idx++) {
					bool isAvailable = random.Next (0, 10) > 3;
					items [idx] = new ViewPagerItem (Activity, dt, isAvailable);
					dt = dt.AddDays (1);
				}

				return items;
			}
		}

		/// <summary>
		/// Количество страниц
		/// </summary>
		public static int PagesCount = 0;

		/// <summary>
		/// Не совсем бесконечная прокрутка :) но очень длинная. Количество циклов 1000
		/// </summary>
		public static int LoopsCount = 1000;

		/// <summary>
		/// Вычисляем номер первой страницы
		/// </summary>
		public static int FirstPageNumber = 0;

		private ViewPagerAdapter _pagerAdapter;
		/// <summary>
		/// Адаптер карусели
		/// </summary>
		/// <value>The pager adapter.</value>
		public ViewPagerAdapter PagerAdapter{get{return _pagerAdapter;}}

		private ModelHolderFragment _model;
		/// <summary>
		/// Модель
		/// </summary>
		/// <value>The model.</value>
		public ModelHolderFragment Model{get{return _model;}}

		private ViewPager _pager;
		/// <summary>
		/// Сама карусель
		/// </summary>
		/// <value>The pager.</value>
		public ViewPager Pager{get{return _pager;}}

		private TimetableItemAdapter _timetableAdapter;
		/// <summary>
		/// Расписание
		/// </summary>
		/// <value>The timetable adapter.</value>
		public TimetableItemAdapter TimetableAdapter{get{ return _timetableAdapter;}}

		private ListView _timetableList;

		private Button _bookButton;
		/// <summary>
		/// Кнопка "Записаться"
		/// </summary>
		/// <value>The book button.</value>
		public Button BookButton{get{return _bookButton;}}

		protected override void OnCreate(Bundle savedInstanceState) {
			base.OnCreate(savedInstanceState);
			SetContentView (Resource.Layout.activity_main);

			//инициализируем фрагмент без интерфейса для хранения модели только при первом создании вьюхи
			if (savedInstanceState == null) {
				FragmentManager.BeginTransaction ().Add ((Android.App.Fragment)new ModelHolderFragment (), ModelHolderFragment.SearchTag).Commit();
				FragmentManager.ExecutePendingTransactions ();
			}
			_model = (ModelHolderFragment)FragmentManager.FindFragmentByTag (ModelHolderFragment.SearchTag);

			_bookButton = FindViewById<Button> (Resource.Id.button_book);

			SetupViewPager ();
			SetupTimetable ();

			_pagerAdapter.UpdateTimetable ();
		}

		/// <summary>
		/// Создание и настройка карусели
		/// </summary>
		private void SetupViewPager(){
			_pager = FindViewById<ViewPager>(Resource.Id.view_pager);

			_pagerAdapter = new ViewPagerAdapter (this, this.SupportFragmentManager, _model.DataSourceItems);
			_pager.Adapter = _pagerAdapter;
			_pager.SetOnPageChangeListener(_pagerAdapter);

			//устанавливаем текущий элемент в пейджере на середину, что бы можно было прокручивать в обе стороны
			_pager.SetCurrentItem(FirstPageNumber, true);

			//количество страниц, которые будут ретейниться контролом единовременно
			_pager.OffscreenPageLimit = 10;

			//установим расстояние между страницами в отрицательное значение, что бы уплотнить страницы
			_pager.PageMargin = -500;

			FindViewById<ImageButton> (Resource.Id.viewpager_button_previous).Click += delegate {
				_pagerAdapter.PrevItem();
			};
			FindViewById<ImageButton> (Resource.Id.viewpager_button_next).Click += delegate {
				_pagerAdapter.NextItem();
			};
		}

		/// <summary>
		/// Создание и настройка связанного расписания
		/// </summary>
		void SetupTimetable ()
		{
			_timetableList = FindViewById<ListView> (Resource.Id.list_view_timetable);
			_timetableAdapter = new TimetableItemAdapter (this, Resource.Layout.timetable_list_item, new TimetableItem[0]);
			_timetableList.Adapter = _timetableAdapter;
		}
	}
}


