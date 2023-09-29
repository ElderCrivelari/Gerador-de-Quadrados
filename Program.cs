using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace GeradorQuadrados
{
    internal class Program
    {
        static void Main()
        {
            int canvasWidth = 800; // Largura do canvas
            int canvasHeight = 800; // Altura do canvas
            int squareSize = 32; // Tamanho de cada quadrado
            int borderSize = 10; // Tamanho da borda

            int maxQuad = 40; // Número máximo de quadrados a serem gerados
            int maxSeq = 5; // Máximo de quadrados em sequência em uma fileira

            // Inicialize o canvas
            Bitmap canvas = new Bitmap(canvasWidth, canvasHeight);

            // Lista de quadrados gerados
            List<Rectangle> squares = new List<Rectangle>();

            Random random = new Random();
            Rectangle square = Rectangle.Empty; // Inicialização com Rectangle.Empty

            for (int i = 0; i < maxQuad; i++)
            {
                if (squares.Count == 0)
                {
                    // Gera o primeiro quadrado no centro do canvas em vermelho com borda azul
                    int x = (canvasWidth - squareSize) / 2;
                    int y = (canvasHeight - squareSize) / 2;
                    square = new Rectangle(x, y, squareSize, squareSize);
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(Brushes.Red, square);
                        g.DrawRectangle(Pens.Blue, x, y, squareSize, squareSize); // Adiciona a borda azul
                        DrawNumber(g, i + 1, square); // Adiciona o número ao quadrado
                    }
                }
                else
                {
                    // Restante do código para gerar novos quadrados em branco com borda azul e sem sobreposição
                    Rectangle lastSquare = squares[squares.Count - 1];

                    // Calcula as posições possíveis sem sobreposição com a restrição de maxSeq
                    List<Point> possiblePositions = CalculatePossiblePositions(lastSquare, squareSize, borderSize, canvasWidth, canvasHeight, squares, maxSeq);

                    if (possiblePositions.Count == 0)
                    {
                        // Se não houver posições válidas, recomece a partir do primeiro quadrado
                        i = 0;
                        squares.Clear();
                        continue;
                    }

                    // Escolha uma posição aleatória entre as posições válidas
                    Point position = possiblePositions[random.Next(possiblePositions.Count)];
                    square = new Rectangle(position.X, position.Y, squareSize, squareSize);
                    using (Graphics g = Graphics.FromImage(canvas))
                    {
                        g.FillRectangle(Brushes.White, square); // Preencha o quadrado em branco
                        g.DrawRectangle(Pens.Blue, position.X, position.Y, squareSize, squareSize); // Adiciona a borda azul
                        DrawNumber(g, i + 1, square); // Adiciona o número ao quadrado
                    }
                }

                // Adicione o quadrado à lista
                squares.Add(square);
            }

            // Salve o canvas como imagem
            string filename = "output.png";
            canvas.Save(filename);
            Process.Start("cmd", $"/c start {filename}");
        }

        // Função para calcular posições possíveis sem sobreposição
        static List<Point> CalculatePossiblePositions(Rectangle lastSquare, int squareSize, int borderSize, int canvasWidth, int canvasHeight, List<Rectangle> existingSquares, int maxSeq)
        {
            List<Point> possiblePositions = new List<Point>();

            // Determine as posições possíveis (topo, baixo, esquerda, direita)
            Point[] directions = {
                new Point(-squareSize - borderSize, 0), // Esquerda
                new Point(squareSize + borderSize, 0), // Direita
                new Point(0, -squareSize - borderSize), // Topo
                new Point(0, squareSize + borderSize) // Baixo
            };

            foreach (Point direction in directions)
            {
                Point newPosition = new Point(lastSquare.X + direction.X, lastSquare.Y + direction.Y);
                Rectangle newSquare = new Rectangle(newPosition.X, newPosition.Y, squareSize, squareSize);

                bool isValidPosition = true;

                // Verifique se a nova posição se sobrepõe a algum quadrado existente
                foreach (Rectangle existingSquare in existingSquares)
                {
                    if (newSquare.IntersectsWith(existingSquare))
                    {
                        isValidPosition = false;
                        break;
                    }
                }

                // Verifique se a nova posição está dentro dos limites do canvas
                if (newSquare.Left < 0 || newSquare.Right > canvasWidth || newSquare.Top < 0 || newSquare.Bottom > canvasHeight)
                {
                    isValidPosition = false;
                }

                // Verifique o tamanho da sequência em uma fileira
                int seqCount = 0;
                foreach (Rectangle existingSquare in existingSquares)
                {
                    if (existingSquare.Top == newSquare.Top && existingSquare.Bottom == newSquare.Bottom)
                    {
                        seqCount++;
                        if (seqCount >= maxSeq)
                        {
                            isValidPosition = false;
                            break;
                        }
                    }
                    else
                    {
                        seqCount = 0;
                    }
                }

                if (isValidPosition)
                {
                    possiblePositions.Add(newPosition);
                }
            }

            return possiblePositions;
        }

        // Função para desenhar um número no centro do quadrado
        static void DrawNumber(Graphics g, int number, Rectangle square)
        {
            using (Font font = new Font("Arial", 12))
            {
                SizeF textSize = g.MeasureString(number.ToString(), font);
                float x = square.Left + (square.Width - textSize.Width) / 2;
                float y = square.Top + (square.Height - textSize.Height) / 2;
                g.DrawString(number.ToString(), font, Brushes.Black, x, y);
            }
        }
    }
}
