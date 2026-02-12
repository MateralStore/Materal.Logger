using Microsoft.VisualStudio.TestTools.UnitTesting;

// ���ò��Բ��л�����߲���ִ������
// Workers = 0 ��ʾʹ�����п��õĴ���������
// Scope = MethodLevel ��ʾ�ڷ���������ִ�в���
[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]
