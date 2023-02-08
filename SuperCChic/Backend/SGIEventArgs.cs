using Backend.Models;

namespace Backend;


public class SGIEventArgs : EventArgs
{
	public Product? Model { get; private set; }
	public string Value { get; private set; }

	public SGIEventArgs(string value)
	{
		Value = value;
	}

	public SGIEventArgs(string value, Product model)
		:this(value)
	{
		Model = model;
	}
}
