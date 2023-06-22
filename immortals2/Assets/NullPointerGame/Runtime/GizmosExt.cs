using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Extension for the Gizmos already provided by unity
	/// </summary>
	public class GizmosExt
	{
		public static Color orange = RGBHexColorToColor(0xFFD692);
		public static Color chocolate = RGBHexColorToColor(0xD2691E);

		// Green based
		public static Color greenYellow = RGBHexColorToColor(0xADFF2F);
		public static Color lawnGreen = RGBHexColorToColor(0x7CFC00);
		public static Color lime = RGBHexColorToColor(0x00FF00);
		public static Color limeGreen = RGBHexColorToColor(0x32CD32);
		public static Color paleGreen = RGBHexColorToColor(0x98FB98);

		// cyan based
		public static Color lightCyan = RGBHexColorToColor(0xC0FFFF);
		public static Color cyan = RGBHexColorToColor(0x00FFFF);
		public static Color aquaMarine = RGBHexColorToColor(0x7FFFD4);
		public static Color turquoise = RGBHexColorToColor(0x40E0D0);
		

		public static Color RGBHexColorToColor(int clr)
		{
			return new Color( (clr>>16)/255.0f, ((clr>>8)&0xff)/255.0f, (clr & 0xff) / 255.0f);
		}

		/// <summary>
		/// Draws a wired circle with a center and a certain radius. <br />
		/// The circle will be projected in the XZ plane. <br />
		/// The number of sides of the circle can also be configured. Default: 24
		/// </summary>
		/// <param name="center">The center point of the circle in world coordinates.</param>
		/// <param name="radius">The radius for the circle.</param>
		/// <param name="sides">The number of sides that will have the shape, default: 24.</param>
		static public void DrawWireCircle( Vector3 center, float radius, int sides = 24 )
		{
			if(sides < 3) sides = 3;
			float theta_scale = (2.0f * Mathf.PI) / sides;

			Vector3 firstPos = new Vector3(radius*Mathf.Cos(0), 0, radius*Mathf.Sin(0));
			Vector3 lastPos = firstPos;
			Vector3 nextpos = new Vector3(0, 0, 0);
		
			for(float theta = theta_scale; theta <= 2 * Mathf.PI; theta += theta_scale)
			{
				nextpos.x = radius*Mathf.Cos(theta);
				nextpos.z = radius*Mathf.Sin(theta);
			
				Gizmos.DrawLine( center+lastPos, center+nextpos );
				lastPos = nextpos;
			}
			Gizmos.DrawLine( nextpos, firstPos );
		}

#if UNITY_EDITOR
		public static GUIStyle debugStyle = null;
		public static GUIStyle DebugStyle {
			get
			{
				if (debugStyle == null)
				{
					debugStyle = new GUIStyle(GUI.skin.box);
					debugStyle.alignment = TextAnchor.MiddleCenter;
					debugStyle.fontSize = 10;
					debugStyle.normal.background = Texture2D.whiteTexture;
					debugStyle.normal.textColor = Color.green;
					debugStyle.margin.top = debugStyle.margin.bottom = 0;
					debugStyle.padding.top = debugStyle.padding.bottom = 0;
				}
				return debugStyle;
			}
		}

		public static void Label(Vector3 pos, string content)
		{
			GUI.backgroundColor = Color.black;
			UnityEditor.Handles.color = Color.black;
			UnityEditor.Handles.Label(pos, content, DebugStyle);

		}
#endif
	}
}