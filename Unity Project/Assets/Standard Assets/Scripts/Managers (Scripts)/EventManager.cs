using System;
using UnityEngine;
using System.Collections.Generic;

namespace Frogger
{
	public class EventManager : SingletonUpdateWhileEnabled<EventManager>
	{
		public static List<Event> events = new List<Event>();

		public override void DoUpdate ()
		{
			for (int i = 0; i < events.Count; i ++)
			{
				Event _event = events[i];
				if (Time.time >= _event.time)
				{
					_event.onEvent (_event.arg);
					events.RemoveAt(i);
					if (events.Count == 0)
					{
						enabled = false;
						return;
					}
					i --;
				}
			}
		}

		public static Event AddEvent (Action<object> action, float time, object arg = null)
		{
			Event event_ = new Event(action, time, arg);
			events.Add(event_);
			Instance.enabled = true;
			return event_;
		}

		public static bool RemoveEvent (Event _event)
		{
			int eventIndex = events.IndexOf(_event);
			if (eventIndex == -1)
				return false;
			events.RemoveAt(eventIndex);
			return true;
		}

		public class Event
		{
			public Action<object> onEvent;
			public float time;
			public object arg;

			public Event (Action<object> onEvent, float time, object arg)
			{
				this.onEvent = onEvent;
				this.time = time;
				this.arg = arg;
			}

			public Event (Event _event) : this (_event.onEvent, _event.time, _event.arg)
			{
			}
		}
	}
}