using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A color mixer represents a gradient color set.
/// Given a number t between 0 and 1, the output color c(t) is linearly interpolated between the 2 neighboring color nodes
/// </summary>
public class ColorMixer {

	private struct ColorNode {
		public Color color;
		public float coeff;
		public ColorNode(Color color, float coeff) {
			this.color = color;
			this.coeff = coeff;
		}
	}
	private LinkedList<ColorNode> colors;
	public enum ExtendMode
	{
		NORMAL,
		REPEAT,
		REPEAT_INVERSE,
	}
	public ExtendMode extendMode = ExtendMode.NORMAL;

	//if do not have input parameter -> ExtendMode.NORMAL
	public ColorMixer(ExtendMode extendMode = ExtendMode.NORMAL) {
		colors = new LinkedList<ColorNode>();
		this.extendMode = extendMode;
	}

	/// <summary>
	/// Inserts a color node at certain position.
	/// </summary>
	public void InsertColorNode(Color color, float coeff) {
		LinkedListNode<ColorNode> current = colors.First;
		while (current != null && current.Value.coeff < coeff) {
			current = current.Next;
		}
		if (current == null)
			colors.AddLast(new ColorNode(color, coeff));
		else
			colors.AddBefore(current, new ColorNode(color, coeff));
	}

	/// <summary>
	/// Returns the color at certain position.
	/// Value should be between 0 and 1.
	/// </summary>
	public Color GetColor(float value) {
		if (colors.Count == 0) return new Color(0,0,0,0);
		if (value < 0 || value > 1) {
			switch (extendMode) {
			case ExtendMode.REPEAT:
				value = (value % 1 + 1) % 1;
				break;
			case ExtendMode.REPEAT_INVERSE:
				value = Mathf.Abs(((value + 1) % 2 + 2) % 2 - 1);
				break;
			}
		}
		LinkedListNode<ColorNode> current = colors.First;
		while (current != null && current.Value.coeff < value) {
			current = current.Next;
		}
		if (current == null)
			return colors.Last.Value.color;
		if (current.Previous == null)
			return colors.First.Value.color;
		ColorNode left = current.Previous.Value;
		ColorNode right = current.Value;
		return Color.Lerp(left.color, right.color, (value - left.coeff) / (right.coeff - left.coeff));
	}
}
