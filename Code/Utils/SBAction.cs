using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace SoulsBox
{
	public class SBAction
	{
		public string ActionString { get; set; }
		public string DisplayText { get; set; }
		public InputGlyphSize GlyphSize { get; set; } = InputGlyphSize.Small;
		public bool Outline { get; set; } = false;

		public Texture GetTexture()
		{
			return Input.GetGlyph(ActionString, GlyphSize, Outline);
		}
	}
}
