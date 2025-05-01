﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RGames.Infrastucture
{
    public enum MessengerMode
    {
        DONT_REQUIRE_LISTENER,
        REQUIRE_LISTENER,
    }

    static internal class MessengerInternal
    {
        readonly internal static Dictionary<string, Delegate> eventTable = new Dictionary<string, Delegate>();
        static internal readonly MessengerMode DEFAULT_MODE = MessengerMode.REQUIRE_LISTENER;

        static internal void AddListener(string eventType, Delegate callback)
        {
            MessengerInternal.OnListenerAdding(eventType, callback);
            eventTable[eventType] = Delegate.Combine(eventTable[eventType], callback);
        }

        static internal void RemoveListener(string eventType, Delegate handler)
        {
            MessengerInternal.OnListenerRemoving(eventType, handler);
            eventTable[eventType] = Delegate.Remove(eventTable[eventType], handler);
            MessengerInternal.OnListenerRemoved(eventType);
        }

        static internal T[] GetInvocationList<T>(string eventType)
        {
            Delegate d;
            if (eventTable.TryGetValue(eventType, out d))
            {
                if (d != null)
                {
                    return d.GetInvocationList().Cast<T>().ToArray();
                }
                else
                {
                    throw MessengerInternal.CreateBroadcastSignatureException(eventType);
                }
            }
            return null;
        }

        static internal void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
        {
            if (!eventTable.ContainsKey(eventType))
            {
                eventTable.Add(eventType, null);
            }

            var d = eventTable[eventType];
            if (d != null && d.GetType() != listenerBeingAdded.GetType())
            {
                throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            }
        }

        static internal void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
        {
            if (eventTable.ContainsKey(eventType))
            {
                var d = eventTable[eventType];

                if (d == null)
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with for event type {0} but current listener is null.", eventType));
                }
                else if (d.GetType() != listenerBeingRemoved.GetType())
                {
                    throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                }
            }
            else
            {
                throw new ListenerException(string.Format("Attempting to remove listener for type {0} but Messenger doesn't know about this event type.", eventType));
            }
        }

        static internal void OnListenerRemoved(string eventType)
        {
            if (eventTable[eventType] == null)
            {
                eventTable.Remove(eventType);
            }
        }

        static internal void OnBroadcasting(string eventType, MessengerMode mode)
        {
            if (mode == MessengerMode.REQUIRE_LISTENER && !eventTable.ContainsKey(eventType))
            {
                throw new MessengerInternal.BroadcastException(string.Format("Broadcasting message {0} but no listener found.", eventType));
            }
        }

        static internal BroadcastException CreateBroadcastSignatureException(string eventType)
        {
            return new BroadcastException(string.Format("Broadcasting message {0} but listeners have a different signature than the broadcaster.", eventType));
        }

        internal sealed class BroadcastException : Exception
        {
            public BroadcastException(string msg)
                : base(msg)
            {
            }
        }

        internal sealed class ListenerException : Exception
        {
            public ListenerException(string msg)
                : base(msg)
            {
            }
        }
    }


    // No parameters
    /// <summary>
    /// Интерфейс передачи сообщений (MPI)
    /// </summary>
    static public class Messenger
    {
        static public void AddListener(string eventType, Action handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void AddListener<TReturn>(string eventType, Func<TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void RemoveListener(string eventType, Action handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void RemoveListener<TReturn>(string eventType, Func<TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void Broadcast(string eventType)
        {
            //UnityEngine.Debug.Log(eventType);
            Broadcast(eventType, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall)
        {
            Broadcast(eventType, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast(string eventType, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Action>(eventType);

                foreach (var callback in invocationList)
                    callback.Invoke();
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }

        static public void Broadcast<TReturn>(string eventType, Action<TReturn> returnCall, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Func<TReturn>>(eventType);

                foreach (var result in invocationList.Select(del => del.Invoke()).Cast<TReturn>())
                {
                    returnCall.Invoke(result);
                }
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }
    }


    // One parameter
    /// <summary>
    /// Интерфейс передачи сообщений (MPI)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public class Messenger<T>
    {
        static public void AddListener(string eventType, Action<T> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void AddListener<TReturn>(string eventType, Func<T, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void RemoveListener(string eventType, Action<T> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void RemoveListener<TReturn>(string eventType, Func<T, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void Broadcast(string eventType, T arg1)
        {
            Broadcast(eventType, arg1, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall)
        {
            Broadcast(eventType, arg1, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast(string eventType, T arg1, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Action<T>>(eventType);

                foreach (var callback in invocationList)
                    callback.Invoke(arg1);
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }

        static public void Broadcast<TReturn>(string eventType, T arg1, Action<TReturn> returnCall, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Func<T, TReturn>>(eventType);

                foreach (var result in invocationList.Select(del => del.Invoke(arg1)).Cast<TReturn>())
                {
                    returnCall.Invoke(result);
                }
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }
    }


    // Two parameters
    /// <summary>
    /// Интерфейс передачи сообщений (MPI)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    static public class Messenger<T, U>
    {
        static public void AddListener(string eventType, Action<T, U> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void AddListener<TReturn>(string eventType, Func<T, U, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void RemoveListener(string eventType, Action<T, U> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void RemoveListener<TReturn>(string eventType, Func<T, U, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void Broadcast(string eventType, T arg1, U arg2)
        {
            Broadcast(eventType, arg1, arg2, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, Action<TReturn> returnCall)
        {
            Broadcast(eventType, arg1, arg2, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast(string eventType, T arg1, U arg2, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Action<T, U>>(eventType);

                foreach (var callback in invocationList)
                    callback.Invoke(arg1, arg2);
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }

        static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, Action<TReturn> returnCall, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Func<T, U, TReturn>>(eventType);

                foreach (var result in invocationList.Select(del => del.Invoke(arg1, arg2)).Cast<TReturn>())
                {
                    returnCall.Invoke(result);
                }
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }
    }


    // Three parameters
    /// <summary>
    /// Интерфейс передачи сообщений (MPI)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    static public class Messenger<T, U, V>
    {
        static public void AddListener(string eventType, Action<T, U, V> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void AddListener<TReturn>(string eventType, Func<T, U, V, TReturn> handler)
        {
            MessengerInternal.AddListener(eventType, handler);
        }

        static public void RemoveListener(string eventType, Action<T, U, V> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void RemoveListener<TReturn>(string eventType, Func<T, U, V, TReturn> handler)
        {
            MessengerInternal.RemoveListener(eventType, handler);
        }

        static public void Broadcast(string eventType, T arg1, U arg2, V arg3)
        {
            Broadcast(eventType, arg1, arg2, arg3, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, V arg3, Action<TReturn> returnCall)
        {
            Broadcast(eventType, arg1, arg2, arg3, returnCall, MessengerInternal.DEFAULT_MODE);
        }

        static public void Broadcast(string eventType, T arg1, U arg2, V arg3, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Action<T, U, V>>(eventType);

                foreach (var callback in invocationList)
                    callback.Invoke(arg1, arg2, arg3);
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }

        static public void Broadcast<TReturn>(string eventType, T arg1, U arg2, V arg3, Action<TReturn> returnCall, MessengerMode mode)
        {
            try
            {
                MessengerInternal.OnBroadcasting(eventType, mode);
                var invocationList = MessengerInternal.GetInvocationList<Func<T, U, V, TReturn>>(eventType);

                foreach (var result in invocationList.Select(del => del.Invoke(arg1, arg2, arg3)).Cast<TReturn>())
                {
                    returnCall.Invoke(result);
                }
            }
            catch
            {
                // Заглушка, для того чтобы код не ломался, если на событие никто не подписан
            }
        }
    }
}
