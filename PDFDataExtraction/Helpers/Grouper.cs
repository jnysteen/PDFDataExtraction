using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFDataExtraction.Helpers
{
    public static class Grouper
    {
        public static List<TGroup> GroupByCondition<TPart, TGroup>(IEnumerable<TPart> partsInExaminationOrder, Func<TPart, TPart, bool> groupingCondition, Func<IEnumerable<TPart>, TGroup> groupingCreator )
        {
            var constructedGroups = new List<TGroup>();

            var partsToBeGrouped = new Queue<TPart>();
            foreach (var partToBePutInGroup in partsInExaminationOrder)
                partsToBeGrouped.Enqueue(partToBePutInGroup);

            var groupUnderConstruction = new Stack<TPart>();

            while (partsToBeGrouped.Any())
            {
                var partToBeExamined = partsToBeGrouped.Dequeue();

                if (!groupUnderConstruction.Any())
                {
                    groupUnderConstruction.Push(partToBeExamined);
                    continue;
                }

                var latestAddedPart = groupUnderConstruction.Peek();

                if (groupingCondition(latestAddedPart, partToBeExamined))
                {
                    groupUnderConstruction.Push(partToBeExamined);
                    continue;
                }

                constructedGroups.Add(groupingCreator(groupUnderConstruction.Reverse()));
                groupUnderConstruction.Clear();
                groupUnderConstruction.Push(partToBeExamined);
            }

            if (groupUnderConstruction.Any())
                constructedGroups.Add(groupingCreator(groupUnderConstruction.Reverse()));

            return constructedGroups;
        }
    }
}