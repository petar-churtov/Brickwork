using System;
using System.Collections.Generic;
using System.Text;

namespace Brickwork
{
    class Program
    {
        public static void Main()
        {
            AreaSize brickworkAreaSize = GetAreaSize();

            if (brickworkAreaSize == null)
            {
                return;
            }

            Layer firstLayer = GetFirstLayer(brickworkAreaSize);

            if (firstLayer == null)
            {
                return;
            }

            bool isFirstLayerValid = IsLayerValidForAreSize(brickworkAreaSize, firstLayer);
            if (!isFirstLayerValid)
            {
                return;
            }

            AddMetadataToFistLayer(firstLayer);

            Layer newLayer = BuildLayer(brickworkAreaSize, firstLayer);

            FillBrickNumbers(firstLayer, newLayer);

            PrintLayer(newLayer);
        }

        private static void FillBrickNumbers(Layer firstLayer, Layer newLayer, Brick currentBrick = null, int brickNumber = 1)
        {
            if (currentBrick == null)
            {
                Brick brick = newLayer.Bricks[0];
                Brick lastBrick = GetLastBrickFromFirstLayerForTheSamePosition(firstLayer, brick);

                brick.IsFirstBrick = true;
                brick.IsHorizontal = !lastBrick.IsHorizontal;
                brick.Number = brickNumber;

                int pairBrickLine = (!brick.IsHorizontal ? brick.Line + 1 : brick.Line);
                int pairBrickColumn = (brick.IsHorizontal ? brick.Column + 1 : brick.Column);

                Brick pairBrick = GetBrickOnPosition(newLayer.Bricks, pairBrickLine, pairBrickColumn);

                pairBrick.IsFirstBrick = false;
                pairBrick.IsHorizontal = brick.IsHorizontal;
                pairBrick.Number = brickNumber;
            }
            else
            {
                currentBrick.IsFirstBrick = true;
                currentBrick.Number = brickNumber;

                Brick previosBrick;
                if (currentBrick.Column == 0)
                {
                    previosBrick = GetBrickOnPosition(newLayer.Bricks, currentBrick.Line - 1, currentBrick.Column);
                }
                else
                {
                    previosBrick = GetBrickOnPosition(newLayer.Bricks, currentBrick.Line, currentBrick.Column - 1);
                }

                currentBrick.IsHorizontal = !previosBrick.IsHorizontal;

                int pairBrickLine = (!currentBrick.IsHorizontal ? currentBrick.Line + 1 : currentBrick.Line);
                int pairBrickColumn = (currentBrick.IsHorizontal ? currentBrick.Column + 1 : currentBrick.Column);

                Brick pairBrick = GetBrickOnPosition(newLayer.Bricks, pairBrickLine, pairBrickColumn);
                
                pairBrick.IsFirstBrick = false;
                pairBrick.IsHorizontal = currentBrick.IsHorizontal;
                pairBrick.Number = brickNumber;
            }

            var nextBrick = FindNextBrickWithoutNumber(newLayer.Bricks);
            if(nextBrick != null)
            {
                int nextBrickNumber = brickNumber + 1;
                FillBrickNumbers(firstLayer, newLayer, nextBrick, nextBrickNumber);
            }
        }

        private static Brick FindNextBrickWithoutNumber(List<Brick> bricks)
        {
            Brick brick = null;

            for (var i = 0; i < bricks.Count; i++)
            {
                Brick tempBrick = bricks[i];
                if (tempBrick.Number == null)
                {
                    brick = tempBrick;
                    break;
                }
            }

            return brick;
        }

        private static Brick GetBrickOnPosition(List<Brick> bricks, int line, int column)
        {
            for (int i = 0; i < bricks.Count; i++)
            {
                Brick brick = bricks[i];
                if (brick.Line == line && brick.Column == column)
                {
                    return brick;
                }
            }

            return null;
        }

        private static bool IsLayerValidForAreSize(AreaSize brickworkAreaSize, Layer layer)
        {
            bool isLayerValid = true;
            if ((brickworkAreaSize.Columns * brickworkAreaSize.Lines) != layer.Bricks.Count)
            {
                Console.WriteLine("First layer columns and lines mismatch the brick count.");
            }

            for (int i = 0; i < layer.Bricks.Count; i++)
            {
                Brick brick = layer.Bricks[i];

                bool isBrickValid = IsBrickValid(layer.Bricks, brick);
                if (!isBrickValid)
                {
                    isLayerValid = false;
                    break;
                }
            }

            if (!isLayerValid)
            {
                Console.WriteLine("First layer has invalid data.");
            }

            return isLayerValid;
        }

        private static bool IsBrickValid(List<Brick> bricks, Brick currentBrick)
        {

            List<Brick> allElementsOfBrick = new List<Brick>();

            for (int i = 0; i < bricks.Count; i++)
            {
                Brick brick = bricks[i];
                if (brick.Number == currentBrick.Number && !(brick.Line == currentBrick.Line && brick.Column == currentBrick.Column))
                {
                    allElementsOfBrick.Add(brick);
                }
            }

            if (allElementsOfBrick.Count != 1)
            {
                Console.WriteLine("Brick with number {0} has incorrect number of values.", currentBrick.Number);
                return false;
            }

            Brick pairBrick = allElementsOfBrick[0];

            if (pairBrick.Column == currentBrick.Column || pairBrick.Line == currentBrick.Line)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Brick with number {0} has incorrect number of values.", currentBrick.Number);
                return false;
            }
        }

        private static AreaSize GetAreaSize()
        {
            Console.WriteLine("Enter lines and columns for the Area N M");
            string userInput = Console.ReadLine().Trim();
            string[] inputParts = userInput.Split(" ");

            if (inputParts.Length != 2)
            {
                Console.WriteLine("Incorrect parameters");
                return null;
            }

            AreaSize brickworkAreaSize = new AreaSize();

            for (int i = 0; i < inputParts.Length; i++)
            {
                int intValue;

                if (!int.TryParse(inputParts[i], out intValue))
                {
                    Console.WriteLine("Incorrect parameters - the value is not a number: " + inputParts[i]);
                    return null;
                }

                if (intValue >= 100)
                {
                    Console.WriteLine("Incorrect parameters - the value is bigger than 100: " + inputParts[i]);
                    return null;
                }

                if (intValue % 2 != 0)
                {
                    Console.WriteLine("Incorrect parameters - the value is odd: " + inputParts[i]);
                    return null;
                }

                if (i == 0)
                {
                    brickworkAreaSize.Lines = intValue;
                }

                if (i == 1)
                {
                    brickworkAreaSize.Columns = intValue;
                }
            }

            return brickworkAreaSize;
        }

        private static Layer GetFirstLayer(AreaSize areaSize)
        {
            Console.WriteLine("Enter first layer");

            Layer firstLayer = new Layer();

            for (int n = 0; n < areaSize.Lines; n++)
            {
                string userInput = Console.ReadLine().Trim();
                string[] inputParts = userInput.Split(" ");

                if (inputParts.Length != areaSize.Columns)
                {
                    Console.WriteLine("Incorrect columns parts");
                    return null;
                }

                for (int m = 0; m < inputParts.Length; m++)
                {
                    int intValue;

                    if (!int.TryParse(inputParts[m].ToString(), out intValue))
                    {
                        Console.WriteLine("Incorrect column value " + inputParts[m]);
                        return null;
                    }

                    Brick brick = new Brick();
                    brick.Line = n;
                    brick.Column = m;
                    brick.Number = intValue;


                    firstLayer.Bricks.Add(brick);
                }

            }

            return firstLayer;
        }

        private static void AddMetadataToFistLayer(Layer firstLayer)
        {
            for (int i = 0; i < firstLayer.Bricks.Count; i++)
            {
                Brick brick = firstLayer.Bricks[i];

                Brick pairBrick = GetPairBrick(firstLayer.Bricks, brick);

                brick.IsFirstBrick = brick.Line < pairBrick.Line || brick.Column < pairBrick.Column;
                brick.IsHorizontal = brick.Line == pairBrick.Line;
            }
        }

        private static Brick GetPairBrick(List<Brick> bricks, Brick currentBrick)
        {
            List<Brick> allElementsOfBrick = new List<Brick>();

            for (int i = 0; i < bricks.Count; i++)
            {
                Brick brick = bricks[i];
                if (brick.Number == currentBrick.Number && !(brick.Line == currentBrick.Line && brick.Column == currentBrick.Column))
                {
                    allElementsOfBrick.Add(brick);
                }
            }

            return allElementsOfBrick[0];
        }

        private static Layer BuildLayer(AreaSize brickworkAreaSize, Layer firstLayer)
        {
            Layer newLayer = new Layer();

            int bricksCount = brickworkAreaSize.Columns * brickworkAreaSize.Lines;

            for (int i = 0; i < bricksCount; i++)
            {
                Brick brick = firstLayer.Bricks[i];

                Brick newBrick = new Brick();

                newBrick.Column = brick.Column;

                newBrick.Line = brick.Line;

                newLayer.Bricks.Add(newBrick);
            }

            return newLayer;
        }

        private static Brick GetLastBrickFromFirstLayerForTheSamePosition(Layer firstLayer, Brick currentBrick)
        {
            Brick lastBrick = null;
            for (var i = 0; i < firstLayer.Bricks.Count; i++)
            {
                var brick = firstLayer.Bricks[i];
                if (brick.Column == currentBrick.Column && brick.Line >= currentBrick.Line)
                {
                    lastBrick = brick;
                }
            }

            return lastBrick;
        }

        private static void PrintLayer(Layer layer)
        {
            StringBuilder consoleOutput = new StringBuilder();
            consoleOutput.AppendLine("----------");
            int currentLine = 0;

            for (int i = 0; i < layer.Bricks.Count; i++)
            {
                Brick brick = layer.Bricks[i];

                if (brick.Line != currentLine)
                {
                    consoleOutput.AppendLine();
                    currentLine = brick.Line;
                }

                if (brick.Column == 0)
                {
                    consoleOutput.Append(brick.Number);
                }
                else
                {
                    consoleOutput.AppendFormat(" {0}", brick.Number);
                }
            }

            Console.WriteLine(consoleOutput.ToString());
        }
    }
}