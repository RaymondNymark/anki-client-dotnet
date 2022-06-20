using AnkiWeb.Client.Common.Models;

namespace AnkiWeb.Client.Tests.Common
{
    public class Card_Tests
    {
        public Card_Tests()
        {
            
        }

        [Fact]
        public void Card_Encoded_Data_Property_Returns_Correctly_Encoded_Values()
        {
            List<Field> Fields = new()
            {
                 new Field()
                    {
                        Name = "Field_One",
                        Value = @"Field one value"
                    },
                    new Field()
                    {
                        Name = "Field_Two",
                        Value = @"Field <b>two<b>"
                    },
                    new Field()
                    {
                        Name = "Field_Three",
                        Value = @""
                    },
            };

            Card card = new("", Fields, "tagOne;tagTwo");

            string expected = @"[[""Field one value"",""Field <b>two<b>"",""""], ""tagOne;tagTwo""]";
            string actual = card.EncodedData;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Card_Encoded_Data_Property_Returns_Correctly_Encoded_Null_Values()
        {
            List<Field> Fields = new()
            {
                 new Field()
                    {
                        Value = @"一番な好きな魚は："
                    },
                    new Field()
                    {
                        Value = @"烏賊\\と//'''ツナ"
                    },
                    new Field()
                    {
                        Value = null
                    }
            };

            Card card = new("", Fields, "tagOne;tagTwo");

            string expected = @"[[""一番な好きな魚は："",""烏賊\\と//'''ツナ"",""""], ""tagOne;tagTwo""]";
            string actual = card.EncodedData;

            Assert.Equal(expected, actual);
        }
    }
}
