

[System.Serializable]
public struct GameDate_Struct
{
    private int m_week;

    public int Week
    {
        readonly get { return m_week; }
        set
        {
            m_week = value;
            if (m_week > 4)
            {
                m_week = 1;
                Month += 1;
            }
        }
    }

    private Months m_month;

    public Months Month
    {
        readonly get { return m_month; }
        set
        {
            m_month = value;
            if (m_month > (Months)12)
            {
                m_month = (Months)1;
                Year += 1;
            }
        }
    }

    private int m_year;

    public int Year
    {
        readonly get { return m_year; }
        set
        {
            m_year = value;
        }
    }

    public GameDate_Struct(int _week, Months _month, int _year)
    {
        m_week = _week;
        m_month = _month;
        m_year = _year;
    }
}


public enum Months
{
    JAN = 1,
    FEV = 2,
    MAR = 3,
    ABR = 4,
    MAIO = 5,
    JUN = 6,
    JUL = 7,
    AGO = 8,
    SET = 9,
    OUT = 10,
    NOV = 11,
    DEZ = 12
}
