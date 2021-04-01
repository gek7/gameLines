using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;

namespace ComponentLines
{
    public class Lines : Control
    {
        //-----------------------
        public delegate void scoreHandler();
        protected int _countBall;
        protected int _score;
        protected int _sizeField;
        protected byte[,] _masCells;
        protected Point selPoint;
        protected event scoreHandler ScoreEdited;
        const int emptyCellValue = 0;
        //-----------------------
        Dictionary<byte, Brush> allColors = new Dictionary<byte, Brush>
        {
            {1, Brushes.Red }, {2, Brushes.Blue},
            {3, Brushes.Purple }, {4, Brushes.Green },
            {5, Brushes.Orange }
        };



        public Lines() : base()
        {
            //Убирает мерцание
            this.DoubleBuffered = true;
            Width = 301;
            Height = 301;
            selPoint.X = -1;
            selPoint.Y = -1;
            _score = 0;
            _countBall = 0;
            _sizeField = 10;
            _masCells = new byte[_sizeField, _sizeField];
            MouseClick += Lines_MouseClick;
            //Ставим первые три шара
            SetBalls();
            Invalidate();
        }
        //Обработчик клика по компоненту
        private void Lines_MouseClick(object sender, MouseEventArgs e)
        {
            SelectBall(e.Location.X, e.Location.Y);
            if (countBall >= _sizeField * _sizeField)
            {
                MessageBox.Show("Игра закончена");
                StartNewGame();
            }
        }

        //Старт новой игры
        public void StartNewGame()
        {
            _masCells = new byte[_sizeField, _sizeField];
            score = 0;
            countBall = 0;
            SetBalls();
        }

        public event scoreHandler onScoreEdited
        {
            add
            {
                ScoreEdited += value;
            }
            remove
            {
                ScoreEdited -= value;
            }
        }

        public virtual int score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                ScoreEdited?.Invoke();
            }

        }

        public virtual int sizeField
        {
            get
            {
                return _sizeField;
            }
            set
            {
                if (value != sizeField)
                {
                    _sizeField = value;
                    StartNewGame();
                }
            }
        }

        public virtual int countBall
        {
            get
            {
                return _countBall;
            }
            set
            {
                if (value != countBall)
                {
                    _countBall = value;
                }
            }
        }

        public new virtual int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;
                Invalidate();

            }
        }

        public new virtual int Height
        {
            get
            {
                return base.Height;
            }
            set
            {
                base.Height = value;
                Invalidate();
            }
        }

        //Проверка 5+ шариков в ряд
        public bool removeStackIfExists()
        {
            List<Point> lst = new List<Point>();
            List<Point> buf = new List<Point>();
            //ToDo может быть ситуация, когда шарики стоят наискосок и соединив их посередине посчитается неправильно
            bool result = false;
            for (int i = 0; i < _masCells.GetLength(0); i++)
            {
                byte elRow = _masCells[i, 0];
                byte elCol = _masCells[0, i];
                byte cntRow = 0;
                byte cntCol = 0;
                for (int j = 0; j < _masCells.GetLength(0); j++)
                {
                    //Удаление в строчке
                    if (elRow == _masCells[i, j] && elRow > 0)
                    {
                        cntRow++;
                    }
                    else
                    {
                        //Удаление 5 и более шариков в ряд
                        if (cntRow >= 5)
                        {
                            //score = score + cntRow;
                            for (int c = j - 1; cntRow > 0; cntRow--, c--)
                            {
                                //_masCells[i, c] = 0;
                                lst.Add(new Point(i, c));
                            }
                            result = true;
                        }
                        elRow = _masCells[i, j];
                        cntRow = 1;
                    }


                    //Удаление в колонке
                    if (elCol == _masCells[j, i] && elCol > 0)
                    {
                        cntCol++;
                    }
                    else
                    {
                        //Удаление 5 и более шариков в ряд
                        if (cntCol >= 5)
                        {
                            //score = score + cntCol;
                            for (int c = j - 1; cntCol > 0; cntCol--, c--)
                            {
                                //_masCells[c, i] = 0;
                                lst.Add(new Point(c, i));
                            }
                            result = true;
                        }
                        elCol = _masCells[j, i];
                        cntCol = 1;
                    }
                }

                //Проверка в конце строки
                if (cntRow >= 5)
                {
                    for (int c = _masCells.GetLength(0) - 1; cntRow > 0; cntRow--, c--)
                    {
                        //_masCells[i, c] = 0;
                        lst.Add(new Point(i, c));
                    }
                    result = true;
                }

                //Проверка в конце колонки
                if (cntCol >= 5)
                {
                    for (int c = _masCells.GetLength(0) - 1; cntCol > 0; cntCol--, c--)
                    {
                        //_masCells[c, i] = 0;
                        lst.Add(new Point(c, i));
                    }
                    result = true;
                }
            }

            
            //////Удаление//////
            while (lst.Count > 0)
            {
                Point p = lst[lst.Count - 1];
                _masCells[p.X, p.Y] = 0;
                lst.RemoveAt(lst.Count - 1);
                countBall--;
                score++;
            }
            return result;
        }

        //Установка 3-х шаров в случайных местах
        public virtual void SetBalls()
        {
            Random r1 = new Random();
            int i = 0;
            while (i < 3 && countBall < (_sizeField * _sizeField))
            {
                byte NumRandom = (byte)r1.Next(1, allColors.Count);
                byte NumRandom2 = (byte)r1.Next(_sizeField);
                byte NumRandom3 = (byte)r1.Next(_sizeField);
                if (_masCells[NumRandom2, NumRandom3] == emptyCellValue)
                {
                    _masCells[NumRandom2, NumRandom3] = NumRandom;
                    countBall++;
                    i++;
                }
            }
            Invalidate();
        }

        //Выбор или перемещение шара
        public virtual void SelectBall(int x, int y)
        {
            //Смещение для правильного нахождения ячейки
            int offset = (sizeField / 2);
            //x и y кликнутой ячейки
            int xp = x / ((Width - offset) / sizeField);
            int yp = y / ((Height - offset)/sizeField);
            if (xp < sizeField && yp < sizeField)
            {
                //Значение выбраной ячейки
                int value = _masCells[xp, yp];

                //Ячейка пустая (Перемещение точки)
                if (value == 0)
                {
                    if (selPoint.X != -1)
                    {
                        //Перемещение точки
                        if (checkPath(selPoint, new Point(xp, yp)))
                        {
                            byte oldVal = _masCells[selPoint.X, selPoint.Y];
                            _masCells[selPoint.X, selPoint.Y] = 0;
                            _masCells[xp, yp] = oldVal;
                            if (!removeStackIfExists()) SetBalls();
                            removeStackIfExists();
                            selPoint.X = -1;
                            selPoint.Y = -1;
                            Invalidate();
                        }
                    }
                }
                //Ячейка не пустая (перевыбор точки)
                else
                {
                    Point p = selPoint;
                    selPoint.X = xp;
                    selPoint.Y = yp;
                    Invalidate();
                }
            }
        }

        //Проверка пути до нового места
        public bool checkPath(Point src, Point dst)
        {
            List<Point> checkedPoints = new List<Point>();
            bool buf = checkPathInner(src); ;
            return buf;

            ////Вложенная функция для рекурсии////
            bool checkPathInner(Point curP)
            {
                checkedPoints.Add(curP);
                Invalidate();
                int x = curP.X - 1;
                int y = curP.Y;
                bool flag = false;
                //Влево
                if (x >= 0 && _masCells[x, y] == 0 && !checkedPoints.Contains(new Point(x, y)))
                {
                    if ((x != dst.X || y != dst.Y) && !flag)
                    {
                        flag = checkPathInner(new Point(x, y));
                    }
                    else
                    {
                        flag = true;
                    }
                }
                x = curP.X + 1;
                //Вправо
                if (x <= _masCells.GetLength(1) - 1 && _masCells[x, y] == 0 && !checkedPoints.Contains(new Point(x, y)))
                {
                    if ((x != dst.X || y != dst.Y) && !flag)
                    {
                        flag = checkPathInner(new Point(x, y));
                    }
                    else
                    {
                        return true;
                    }
                }
                x = curP.X;
                y = curP.Y - 1;
                //Вверх
                if (y >= 0 && _masCells[x, y] == 0 && !checkedPoints.Contains(new Point(x, y)))
                {
                    if ((y != dst.Y || x != dst.X) && !flag)
                    {
                        flag = checkPathInner(new Point(x, y));
                    }
                    else
                    {
                        return true;
                    }
                }
                y = curP.Y + 1;
                //Вниз
                if (y <= _masCells.GetLength(1) - 1 && _masCells[x, y] == 0 && !checkedPoints.Contains(new Point(x, y)))
                {
                    if ((y != dst.Y || x != dst.X) && !flag)
                    {
                        flag = checkPathInner(new Point(x, y));
                    }
                    else
                    {
                        return true;
                    }
                }
                return flag;
            }

        }

        //Перерисовка поля
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var cellSize = new Size((Width - 5) / sizeField, (Height - 5) / sizeField);
            for (int i = 0; i < _masCells.GetLength(0); i++)
            {
                for (int j = 0; j < _masCells.GetLength(1); j++)
                {
                    int x = i * cellSize.Width;
                    int y = j * cellSize.Height;
                    byte value = _masCells[i, j];
                    var rect = new Rectangle(x, y, cellSize.Width, cellSize.Height);
                    e.Graphics.FillRectangle(Brushes.White, rect);
                    e.Graphics.DrawRectangle(Pens.Black, rect);
                    if (value != 0)
                        e.Graphics.FillEllipse(allColors[value], x, y, cellSize.Width, cellSize.Height);

                    //Выделение выбранного шара
                    if (_masCells[i, j] != 0)
                    {
                        if (selPoint.X == i && selPoint.Y == j)
                            e.Graphics.DrawEllipse(new Pen(Brushes.Black, 2), x, y, cellSize.Width, cellSize.Height);
                    }
                }
            }
        }


    }
}
