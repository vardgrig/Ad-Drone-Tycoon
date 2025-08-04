using System;

namespace GameFlow.Currency
{
    [Serializable]
    public class Currency
    {
        public MoneyType moneyType;
        public int amount;
    
        public static Currency operator +(Currency a, Currency b)
        {
            if (a.moneyType != b.moneyType)
                throw new System.ArgumentException("Cannot add different currency types");
        
            return new Currency { moneyType = a.moneyType, amount = a.amount + b.amount };
        }
    
        public static Currency operator -(Currency a, Currency b)
        {
            if (a.moneyType != b.moneyType)
                throw new System.ArgumentException("Cannot subtract different currency types");
        
            return new Currency { moneyType = a.moneyType, amount = a.amount - b.amount };
        }
    
        public static bool operator >=(Currency a, Currency b)
        {
            return a.moneyType == b.moneyType && a.amount >= b.amount;
        }
    
        public static bool operator <=(Currency a, Currency b)
        {
            return a.moneyType == b.moneyType && a.amount <= b.amount;
        }
    }
}