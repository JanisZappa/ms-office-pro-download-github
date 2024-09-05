using System;


public static class timespanExt 
{
	public static float GetSeconds(this TimeSpan timeSpan)
	{
		return (float)timeSpan.TotalSeconds;
	}
}
