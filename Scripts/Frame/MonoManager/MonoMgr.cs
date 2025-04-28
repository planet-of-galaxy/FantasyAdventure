using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 为没有继承MonoBehaviour的类 提供声明周期函数
// 同时提供各种常用协程
/// <summary>
/// 提供方法： （注意：返回协程的方法，需要调用者自己管理协程的生命周期，不要声明了却忘记Stop）
/// DelayInvoke(float delay, Action action) 作用：延迟调用一个委托的方法 返回一个协程对象
/// ChangeFloatGradually(float start, float target, Action<float> action) 作用：让一个float值逐渐变化到目标值 返回一个协程对象
/// ChangeVector3Gradually(Vector3 start, Vector3 target, Action<Vector3> action) 作用：让一个Vector3值逐渐变化到目标值 返回一个协程对象
/// ChangeQuaternionGradually(Quaternion start, Quaternion target, Action<Quaternion> action) 作用：让一个Quaternion值逐渐变化到目标值 返回一个协程对象
/// StartRepeatingAction(float interval, Action action) 作用：每隔interval秒调用一次action 返回一个协程对象
/// </summary>
public class MonoMgr : SingletonMono<MonoMgr>
{
    public event UnityAction update;
    public event UnityAction lateUpdate;
    public event UnityAction fixUpdate;
    public event UnityAction onGUI;

    // Update is called once per frame
    private void Update()
    {
        update?.Invoke();
    }

    private void FixedUpdate()
    {
        fixUpdate?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpdate?.Invoke();
    }

    private void OnGUI()
    {
        onGUI?.Invoke();
    }

    public void AddUpdate(UnityAction fun) {
        update += fun;
    }

    public void RemoveUpdate(UnityAction fun) {
        update -= fun;
    }

    public void AddLateUpdate(UnityAction fun) {
        lateUpdate += fun;
    }

    public void RemoveLateUpdate(UnityAction fun) {
        lateUpdate -= fun;
    }

    public void AddFixUpdate(UnityAction fun)
    {
        fixUpdate += fun;
    }

    public void RemoveFixUpdate(UnityAction fun)
    {
        fixUpdate -= fun;
    }

    public void AddOnGUI(UnityAction fun)
    {
        onGUI += fun;
    }

    public void RemoveOnGUI(UnityAction fun)
    {
        onGUI -= fun;
    }

    // 延迟调用一个委托的方法
    public Coroutine DelayInvoke(float delay, Action action) {
        return StartCoroutine(DelayInvokeCoroutine(delay, action));
    }
    // 让一个float值逐渐变化到目标值
    public Coroutine ChangeFloatGradually(float start, float target, Action<float> action, float scale = 1)
    {
        return StartCoroutine(ChangeFloatGraduallyCorouutine(start, target, action, scale));
    }

    // 让一个Vector3值逐渐变化到目标值
    public Coroutine ChangeVector3Gradually(Vector3 start, Vector3 target, Action<Vector3> action, float scale = 1)
    {
        return StartCoroutine(ChangeVector3GraduallyCorouutine(start, target, action, scale));
    }
    // 让一个Quaternion值逐渐变化到目标值
    public Coroutine ChangeQuaternionGradually(Quaternion start, Quaternion target, Action<Quaternion> action, float scale = 1)
    {
        return StartCoroutine(ChangeQuaternionGraduallyCorouutine(start, target, action, scale));
    }
    // 每隔interval秒调用一次action
    public Coroutine StartRepeatingAction(float interval, Action action)
    {
        return StartCoroutine(RepeatingActionCoroutine(interval, action));
    }
    // float渐变的协程 逻辑实现
    private IEnumerator ChangeFloatGraduallyCorouutine(float start, float target, Action<float> action, float scale)
    {
        while (start != target)
        {
            start = Mathf.Lerp(start, target, Time.deltaTime * scale);
            action?.Invoke(start);
            yield return null;
        }
    }
    // Vector3渐变的协程 逻辑实现
    private IEnumerator ChangeVector3GraduallyCorouutine(Vector3 start, Vector3 target, Action<Vector3> action, float scale)
    {
        while (start != target)
        {
            start = Vector3.Lerp(start, target, Time.deltaTime);
            action?.Invoke(start);
            yield return null;
        }
    }
    // Quaternion渐变的协程 逻辑实现
    private IEnumerator ChangeQuaternionGraduallyCorouutine(Quaternion start, Quaternion target, Action<Quaternion> action, float scale)
    {
        while (start != target)
        {
            start = Quaternion.Slerp(start, target, Time.deltaTime);
            action?.Invoke(start);
            yield return null;
        }
    }
    // 延迟调用的协程 逻辑实现
    private IEnumerator DelayInvokeCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    // 每隔interval秒调用一次action的协程 逻辑实现
    private IEnumerator RepeatingActionCoroutine(float interval, Action action)
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);
            action?.Invoke();
        }
    }

    public void Clear() {
        update = null;
        lateUpdate = null;
        fixUpdate = null;
        onGUI = null;
    }
}
