namespace social.Helper;

public class Helper
{
    public static string CustomUrlEncode(string value)
    {
        char[] temp = Uri.EscapeDataString(value).ToCharArray();

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == '%')
            {
                if (i + 2 < temp.Length && char.IsLower(temp[i + 1]) && char.IsLower(temp[i + 2]))
                {
                    temp[i + 1] = char.ToUpper(temp[i + 1]);
                    temp[i + 2] = char.ToUpper(temp[i + 2]);
                }
            }
        }

        return new string(temp);
    }
}
