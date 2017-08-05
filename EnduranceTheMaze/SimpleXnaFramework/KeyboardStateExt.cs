using Microsoft.Xna.Framework.Input;
using System;

namespace EnduranceTheMaze
{
    /// <summary>
    /// Contains useful utilities.
    /// </summary>
    public static class KeyboardStateExt
    {
        /// <summary>
        /// Returns whether or not the specified key is a letter.
        /// </summary>
        public static bool IsLetter(Keys key)
        {
            if (key == Keys.A ||
                key == Keys.B ||
                key == Keys.C ||
                key == Keys.D ||
                key == Keys.E ||
                key == Keys.F ||
                key == Keys.G ||
                key == Keys.H ||
                key == Keys.I ||
                key == Keys.J ||
                key == Keys.K ||
                key == Keys.L ||
                key == Keys.M ||
                key == Keys.N ||
                key == Keys.O ||
                key == Keys.P ||
                key == Keys.Q ||
                key == Keys.R ||
                key == Keys.S ||
                key == Keys.T ||
                key == Keys.U ||
                key == Keys.V ||
                key == Keys.W ||
                key == Keys.X ||
                key == Keys.Y ||
                key == Keys.Z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether or not the specified key is a digit.
        /// </summary>
        public static bool IsDigit(Keys key)
        {
            if (key == Keys.D0 ||
                key == Keys.D1 ||
                key == Keys.D2 ||
                key == Keys.D3 ||
                key == Keys.D4 ||
                key == Keys.D5 ||
                key == Keys.D6 ||
                key == Keys.D7 ||
                key == Keys.D8 ||
                key == Keys.D9 ||
                key == Keys.NumPad0 ||
                key == Keys.NumPad1 ||
                key == Keys.NumPad2 ||
                key == Keys.NumPad3 ||
                key == Keys.NumPad4 ||
                key == Keys.NumPad5 ||
                key == Keys.NumPad6 ||
                key == Keys.NumPad7 ||
                key == Keys.NumPad8 ||
                key == Keys.NumPad9)
            {
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Returns the proper string representation of visible keys.
        /// </summary>
        /// <param name="shiftPressed">
        /// Whether or not shift is active (not capslock).
        /// </param>
        public static string KeyToString(String str, Keys key,
            bool shiftPressed)
        {
            //Checks for backspace.
            if (key == Keys.Back && str.Length != 0)
            {
                str = str.Remove(str.Length - 1);
            }

            //Checks for letters.
            else if (IsLetter(key))
            {
                if (Console.CapsLock == true || shiftPressed)
                {
                    str += key.ToString();
                }
                else
                {
                    str += key.ToString().ToLower();
                }
            }

            //Checks for keys that have shift-based variants.
            else if (shiftPressed)
            {
                //Digits
                if (key == Keys.D0 || key == Keys.NumPad0) { str += ")"; }
                else if (key == Keys.D1 || key == Keys.NumPad1) { str += "!"; }
                else if (key == Keys.D2 || key == Keys.NumPad2) { str += "@"; }
                else if (key == Keys.D3 || key == Keys.NumPad3) { str += "#"; }
                else if (key == Keys.D4 || key == Keys.NumPad4) { str += "$"; }
                else if (key == Keys.D5 || key == Keys.NumPad5) { str += "%"; }
                else if (key == Keys.D6 || key == Keys.NumPad6) { str += "^"; }
                else if (key == Keys.D7 || key == Keys.NumPad7) { str += "&"; }
                else if (key == Keys.D8 || key == Keys.NumPad8) { str += "*"; }
                else if (key == Keys.D9 || key == Keys.NumPad9) { str += "("; }

                //Punctuation
                else if (key == Keys.OemTilde) { str += "~"; }
                else if (key == Keys.OemOpenBrackets) { str += "{"; }
                else if (key == Keys.OemCloseBrackets) { str += "}"; }
                else if (key == Keys.OemSemicolon) { str += ":"; }
                else if (key == Keys.OemQuotes) { str += "\""; }
                else if (key == Keys.OemComma) { str += "<"; }
                else if (key == Keys.OemPeriod || key == Keys.Decimal)
                    { str += ">"; }
                else if (key == Keys.OemQuestion) { str += "?"; }
                else if (key == Keys.OemMinus) { str += "_"; }
                else if (key == Keys.OemPlus) { str += "+"; }
                else if (key == Keys.OemPipe) { str += "|"; }
            }
            else
            {
                //Digits
                if (key == Keys.D0 || key == Keys.NumPad0) { str += "0"; }
                else if (key == Keys.D1 || key == Keys.NumPad1) { str += "1"; }
                else if (key == Keys.D2 || key == Keys.NumPad2) { str += "2"; }
                else if (key == Keys.D3 || key == Keys.NumPad3) { str += "3"; }
                else if (key == Keys.D4 || key == Keys.NumPad4) { str += "4"; }
                else if (key == Keys.D5 || key == Keys.NumPad5) { str += "5"; }
                else if (key == Keys.D6 || key == Keys.NumPad6) { str += "6"; }
                else if (key == Keys.D7 || key == Keys.NumPad7) { str += "7"; }
                else if (key == Keys.D8 || key == Keys.NumPad8) { str += "8"; }
                else if (key == Keys.D9 || key == Keys.NumPad9) { str += "9"; }

                //Punctuation
                if (key == Keys.OemTilde) { str += "`"; }
                else if (key == Keys.OemOpenBrackets) { str += "["; }
                else if (key == Keys.OemCloseBrackets) { str += "]"; }
                else if (key == Keys.OemSemicolon) { str += ";"; }
                else if (key == Keys.OemQuotes) { str += "'"; }
                else if (key == Keys.OemComma) { str += ","; }
                else if (key == Keys.OemPeriod || key == Keys.Decimal)
                    { str += "."; }
                else if (key == Keys.OemQuestion) { str += "/"; }
                else if (key == Keys.OemMinus) { str += "-"; }
                else if (key == Keys.OemPlus) { str += "="; }
                else if (key == Keys.OemPipe) { str += @"\"; }
            }

            //Miscellaneous keys and punctuation
            if (key == Keys.Space) { str += " "; }
            else if (key == Keys.Divide) { str += "/"; }
            else if (key == Keys.Add) { str += "+"; }
            else if (key == Keys.Subtract) { str += "-"; }
            else if (key == Keys.Multiply) { str += "*"; }

            //If the key pressed was other (like unprintable chars)
            return str;
        }
    }
}