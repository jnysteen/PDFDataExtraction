using System;
using System.Collections.Generic;
using System.Linq;
using PDFDataExtraction.Generic;

namespace PDFDataExtraction.Helpers
{
    public static class FontSizeGroupsCreator
    {
        public static IEnumerable<CharactersFontSizeGroup> FindFontSizeGroups(IEnumerable<Character> charactersToGroup)
        {
            var charactersByHeight = charactersToGroup.OrderBy(c => c.BoundingBox.Height);

            var maxDeviationFactor = 0.05;

            var grouped = Grouper
                .GroupByCondition(charactersByHeight,
                    (thisWord, thatWord) => AcceptableHeightDifference(thisWord, thatWord, maxDeviationFactor),
                    (characters) => new CharactersFontSizeGroup() {Characters = new HashSet<Character>(characters)});

            return grouped;
        }

        public static bool AcceptableHeightDifference(Character characterToCompareTo, Character thisCharacter, double maxDeviationFactor)
        {
            var thisHeight = thisCharacter.BoundingBox.Height;
            var thatHeight = characterToCompareTo.BoundingBox.Height;

            var maxHeightDiff = thisHeight * maxDeviationFactor;

            return Math.Abs(thisHeight - thatHeight) <= maxHeightDiff;
        }
    }
}