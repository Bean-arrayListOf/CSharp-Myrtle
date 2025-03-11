namespace CSharp_Myrtle.Citrus;

public abstract class AutoClosed : IDisposable
{
	private bool _close = true;

	protected AutoClosed()
	{
		AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
	}

	public void Dispose()
	{
		if (_close)
		{
			Close();
			_close = false;
		}
	}

	protected abstract void Close();

	private void OnProcessExit(object? sender, EventArgs e)
	{
		Dispose();
		// 在这里执行清理操作
	}
}