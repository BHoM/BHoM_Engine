using BH.oM.Planning;
using System;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Milestone Milestone(string name, DateTimeOffset dueOn, ItemState state)
        {
            return new Milestone
            {
                Name = name,
                DueOn = dueOn,
                State = state
            };
        }

        /***************************************************/

        public static Milestone Milestone(string name, int year, int month, int day, ItemState state = ItemState.Open)
        {
            DateTimeOffset dto = new DateTimeOffset(year, month, day, 23, 59, 59, TimeSpan.Zero);

            return Create.Milestone(name, dto, state);
        }

        /***************************************************/

    }
}
