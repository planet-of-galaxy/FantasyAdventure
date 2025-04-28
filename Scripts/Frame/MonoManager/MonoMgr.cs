using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Ϊû�м̳�MonoBehaviour���� �ṩ�������ں���
// ͬʱ�ṩ���ֳ���Э��
/// <summary>
/// �ṩ������ ��ע�⣺����Э�̵ķ�������Ҫ�������Լ�����Э�̵��������ڣ���Ҫ������ȴ����Stop��
/// DelayInvoke(float delay, Action action) ���ã��ӳٵ���һ��ί�еķ��� ����һ��Э�̶���
/// ChangeFloatGradually(float start, float target, Action<float> action) ���ã���һ��floatֵ�𽥱仯��Ŀ��ֵ ����һ��Э�̶���
/// ChangeVector3Gradually(Vector3 start, Vector3 target, Action<Vector3> action) ���ã���һ��Vector3ֵ�𽥱仯��Ŀ��ֵ ����һ��Э�̶���
/// ChangeQuaternionGradually(Quaternion start, Quaternion target, Action<Quaternion> action) ���ã���һ��Quaternionֵ�𽥱仯��Ŀ��ֵ ����һ��Э�̶���
/// StartRepeatingAction(float interval, Action action) ���ã�ÿ��interval�����һ��action ����һ��Э�̶���
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

    // �ӳٵ���һ��ί�еķ���
    public Coroutine DelayInvoke(float delay, Action action) {
        return StartCoroutine(DelayInvokeCoroutine(delay, action));
    }
    // ��һ��floatֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeFloatGradually(float start, float target, Action<float> action, float scale = 1)
    {
        return StartCoroutine(ChangeFloatGraduallyCorouutine(start, target, action, scale));
    }

    // ��һ��Vector3ֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeVector3Gradually(Vector3 start, Vector3 target, Action<Vector3> action, float scale = 1)
    {
        return StartCoroutine(ChangeVector3GraduallyCorouutine(start, target, action, scale));
    }
    // ��һ��Quaternionֵ�𽥱仯��Ŀ��ֵ
    public Coroutine ChangeQuaternionGradually(Quaternion start, Quaternion target, Action<Quaternion> action, float scale = 1)
    {
        return StartCoroutine(ChangeQuaternionGraduallyCorouutine(start, target, action, scale));
    }
    // ÿ��interval�����һ��action
    public Coroutine StartRepeatingAction(float interval, Action action)
    {
        return StartCoroutine(RepeatingActionCoroutine(interval, action));
    }
    // float�����Э�� �߼�ʵ��
    private IEnumerator ChangeFloatGraduallyCorouutine(float start, float target, Action<float> action, float scale)
    {
        while (start != target)
        {
            start = Mathf.Lerp(start, target, Time.deltaTime * scale);
            action?.Invoke(start);
            yield return null;
        }
    }
    // Vector3�����Э�� �߼�ʵ��
    private IEnumerator ChangeVector3GraduallyCorouutine(Vector3 start, Vector3 target, Action<Vector3> action, float scale)
    {
        while (start != target)
        {
            start = Vector3.Lerp(start, target, Time.deltaTime);
            action?.Invoke(start);
            yield return null;
        }
    }
    // Quaternion�����Э�� �߼�ʵ��
    private IEnumerator ChangeQuaternionGraduallyCorouutine(Quaternion start, Quaternion target, Action<Quaternion> action, float scale)
    {
        while (start != target)
        {
            start = Quaternion.Slerp(start, target, Time.deltaTime);
            action?.Invoke(start);
            yield return null;
        }
    }
    // �ӳٵ��õ�Э�� �߼�ʵ��
    private IEnumerator DelayInvokeCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    // ÿ��interval�����һ��action��Э�� �߼�ʵ��
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
