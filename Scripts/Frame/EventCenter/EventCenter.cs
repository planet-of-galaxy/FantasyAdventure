using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventInfoBase
{ 
    
}
public class EventInfo : EventInfoBase {
    private event UnityAction acts;

    public void Add(UnityAction act) {
        acts += act;
    }

    public void Remove(UnityAction act) {
        acts -= act;
    }

    public void Invoke() {
        acts?.Invoke();
    }
}
public class EventInfo<T> : EventInfoBase {
    private event UnityAction<T> acts;

    public void Add(UnityAction<T> act) {
        acts += act;
    }

    public void Remove(UnityAction<T> act)
    {
        acts -= act;
    }

    public void Invoke(T obj)
    {
        acts?.Invoke(obj);
    }
}
public class EventCenter : Singleton<EventCenter>
{
    private Dictionary<E_EventType, EventInfoBase> eventCenter = new Dictionary<E_EventType, EventInfoBase>();

    public void Subscribe(E_EventType event_type, UnityAction act) {
        if (!eventCenter.ContainsKey(event_type)) {
            eventCenter.Add(event_type, new EventInfo());
        }
        (eventCenter[event_type] as EventInfo).Add(act);
    }

    public void UnSubscribe(E_EventType event_type, UnityAction act) {
        (eventCenter[event_type] as EventInfo).Remove(act);
    }

    public void EventTrigger(E_EventType event_type) {
        (eventCenter[event_type] as EventInfo)?.Invoke();
    }

    public void Subscribe<T>(E_EventType event_type,UnityAction<T> act) {
        if (!eventCenter.ContainsKey(event_type)) {
            eventCenter.Add(event_type, new EventInfo<T>());
        }
        (eventCenter[event_type] as EventInfo<T>).Add(act);
    }

    public void UnSubscribe<T>(E_EventType event_type, UnityAction<T> act)
    {
        (eventCenter[event_type] as EventInfo<T>).Remove(act);
    }

    public void EventTrigger<T>(E_EventType event_type,T t)
    {
        if(eventCenter.ContainsKey(event_type))
            (eventCenter[event_type] as EventInfo<T>)?.Invoke(t);
    }

    public override void Init()
    {

    }

    public void Clear() {
        eventCenter.Clear();
    }
}