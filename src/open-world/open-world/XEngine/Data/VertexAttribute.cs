﻿using System;

namespace XEngine.Data
{
	[Flags] public enum VertexAttribute
	{
		NONE = 0,
		POSITION = 1,
		COLOR = 2,
		NORMAL = 4,
		ALL = 0x7FFFFFFF
	}
}
