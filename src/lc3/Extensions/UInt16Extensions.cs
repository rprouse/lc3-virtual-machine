namespace LC3.Extensions
{
    public static class UInt16Extensions
    {
        /// <summary>
        /// Takes a number bitCount bits long and extends its two's 
        /// compliment form out to 16 bits.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="bitCount"></param>
        /// <returns></returns>
        public static ushort SignExtend(this ushort x, ushort bitCount)
        {
            if(((x >> (bitCount - 1)) & 1) == 0x01)
            {
                x |= (ushort)(0xFFFF << bitCount);
            }
            return x;
        }

        /// <summary>
        /// Returns the given number of bits from the left/most significant
        /// side of the number.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="bits">The number of bits to take</param>
        /// <returns></returns>
        public static ushort MSB(this ushort x, ushort bits) =>
            (ushort)(x >> (16 - bits));

        /// <summary>
        /// Returns the given number of bits from the right/least significant
        /// side of the number
        /// </summary>
        /// <param name="x"></param>
        /// <param name="bits">The number of bits to take</param>
        /// <returns></returns>
        public static ushort LSB(this ushort x, ushort bits) => 
            (ushort)(x & (0xFFFF >> (16 - bits)));

        /// <summary>
        /// Takes bits from the middle of the number where the least significant
        /// bit is zero indexed up to 15 for the most significant bit
        /// </summary>
        /// <param name="x"></param>
        /// <param name="msb">The first bit to take</param>
        /// <param name="lsb">The last bit to take</param>
        /// <returns></returns>
        public static ushort Bits(this ushort x, ushort msb, ushort lsb)
        {
            ushort mask = (ushort)(0xFFFF >> (15 - msb + lsb));
            return (ushort)((ushort)(x >> lsb) & mask);
        }

        /// <summary>
        /// Swaps Big-Endian to Little-Endian and visa-versa
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ushort Swap16(this ushort x) =>
            (ushort)((x << 8) | (x >> 8));
    }
}
