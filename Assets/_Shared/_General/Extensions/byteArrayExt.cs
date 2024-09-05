public static class byteArrayExt 
{
    public static byte[] Jumble(this byte[] array)
    {
        int count = array.Length;
        
        for (int i = 0; i < count; i++)
        {
            int other = (i + i) % count;

            byte ownByte  = array[i];
            byte swapByte = array[other];

            array[i]     = swapByte;
            array[other] = ownByte;
        }
		
        return array;
    }
	
    public static byte[] UnJumble(this byte[] array)
    {
        int count = array.Length;

        int start = count - 1;
        
        for (int i = start; i > -1; i--)
        {
            int other = (i + i) % count;

            byte ownByte  = array[i];
            byte swapByte = array[other];

            array[i]     = swapByte;
            array[other] = ownByte;
        }
        
        return array;
    }
}