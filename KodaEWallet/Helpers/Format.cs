using System.Globalization;

namespace KodaEWallet.Helpers;
public static class Format
{
    public static string Rupiah(decimal amount)
    {
        string format = amount % 1 == 0 ? "N0" : "N2";
        return "Rp" + amount.ToString(format, CultureInfo.GetCultureInfo("id-ID"));
    }

    public static string DateTimeText(DateTime time) =>
        time.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture);
}