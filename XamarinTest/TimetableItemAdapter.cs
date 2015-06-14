using System;
using System.Collections.Generic;

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
	public class TimetableItemAdapter : ArrayAdapter<TimetableItem>
	{
		/// <summary>
		/// Класс ViewHolder, нужен что бы исключить множественный траверс по элементам вьюхи, для производительности
		/// </summary>
		public class TimetableItemViewHolder : Java.Lang.Object{
			public TextView TimeText { get; private set;}
			public TextView DescriptionText { get; private set;}
			public ImageView AcceptButton { get; private set;}

			public TimetableItem BoundedItem { get; set;}

			public TimetableItemViewHolder(View view, TimetableItem item){
				TimeText = view.FindViewById<TextView>(Resource.Id.timetable_time_text);
				DescriptionText = view.FindViewById<TextView>(Resource.Id.timetable_description_text);
				AcceptButton = view.FindViewById<ImageView>(Resource.Id.timetable_accept);

				BoundedItem = item;
			}
		}

		private MainActivity _context;

		public TimetableItemAdapter(Context context, int resource, IList<TimetableItem> objects) : base(context, resource, objects) {
			_context = (MainActivity)context;
		}
			
		public override View GetView(int position, View convertView, ViewGroup parent) {
			View resultView = null;
			TimetableItemViewHolder holder = null;
			TimetableItem item = GetItem(position);

			if(convertView != null){
				resultView = convertView;
				holder = (TimetableItemViewHolder)convertView.Tag;
				holder.BoundedItem = item;
			}else{
				resultView = ((LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService))
					.Inflate(Resource.Layout.timetable_list_item, parent, false);
				holder = new TimetableItemViewHolder(resultView, item);
				resultView.Tag = holder;

				//регистрируем делегат обработчика только раз - при создании вьюхи
				//так же получаем BoundedItem каждый раз внутри делегата
				resultView.Click += delegate(object sender, EventArgs e) {
					TimetableItemViewHolder itemHolder = (TimetableItemViewHolder)(sender as View).Tag;
					if(itemHolder.BoundedItem.Avability){
						itemHolder.BoundedItem.Booked = !itemHolder.BoundedItem.Booked;

						itemHolder.AcceptButton.Visibility = itemHolder.BoundedItem.Booked ? ViewStates.Visible : ViewStates.Invisible;
						holder.AcceptButton.SetColorFilter(_context.Resources.GetColor (Resource.Color.timetable_accept_button_free));

						//разрешаем/запрещаем нажатие на кнопку "Записаться"
						_context.BookButton.Enabled = itemHolder.BoundedItem.Booked;

						for(int idx = 0; idx < Count; idx++){
							TimetableItem otherItem = GetItem(idx);
							if(otherItem == itemHolder.BoundedItem) continue;

							otherItem.Booked = false;
						}
						NotifyDataSetChanged();
					}
				};
			}

			if(item != null && holder != null){
				holder.TimeText.Text = item.Time;
				holder.DescriptionText.Text = item.Description;

				if(item.Avability) {
					holder.AcceptButton.Visibility = item.Booked ? ViewStates.Visible : ViewStates.Invisible;
					holder.AcceptButton.SetColorFilter(_context.Resources.GetColor (Resource.Color.timetable_accept_button_free));

					holder.TimeText.SetTextColor(_context.Resources.GetColor(Resource.Color.timetable_list_item_available));
					holder.DescriptionText.SetTextColor(_context.Resources.GetColor(Resource.Color.timetable_list_item_available));
					resultView.Enabled = true;
				}else{
					holder.AcceptButton.Visibility = ViewStates.Visible;
					holder.AcceptButton.SetColorFilter(_context.Resources.GetColor (Resource.Color.timetable_accept_button_occupied));

					holder.TimeText.SetTextColor(_context.Resources.GetColor(Resource.Color.timetable_list_item_unavailable));
					holder.DescriptionText.SetTextColor(_context.Resources.GetColor(Resource.Color.timetable_list_item_unavailable));
					resultView.Enabled = false;
				}
			}

			return resultView;
		}
	}
}