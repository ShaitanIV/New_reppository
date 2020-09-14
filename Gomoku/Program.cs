using Gomoku;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gomoku
{

    class Coordinate   
    {
        public int x;  
        public int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
    class Condition  //описание состояние клетки игровой доски
    {
        public int W_High_prior = 1;    // приоритет или количество белых фигур в ряд, которое максимально возможно из всех направлений, если белый игрок поставит фигуру на эту клетку 
        public int W_NE_prior = 1;      // приоритет или количество белых фигур в ряд по диагонали в северо-восточном направлении, если белый игрок поставит фигуру на эту клетку 
        public int W_E_prior = 1;       // в восточном направлении
        public int W_SE_prior = 1;      // в юго-восточном направлении
        public int W_N_prior = 1;       // в северном направлении
        public int B_High_prior = 1;    // аналогично для черного игрока
        public int B_NE_prior = 1;
        public int B_E_prior = 1;
        public int B_SE_prior = 1;
        public int B_N_prior = 1;
        public bool isprotected = false;    // переменные, необходимая для определения необоходимости понижения приоритета для нуля, если для какого-либо игрока существует ситуация, когда на одном участке уже не является
        public bool set_to_0 = false;       //не получится собрать 5 в ряд из-за фигуры врага, но все еще возможно собрать 5 в ряд на другом участке, в который входит данная клетка

        public int piece = 0;


        public void check_for_highest_white()  // после каждого изменение приоритета проверка на наивысший приоритет среди 4 направлений для белого игрока
        {
            if (this.piece == 0)
            {
                this.W_High_prior = this.W_E_prior;
                if (this.W_High_prior < this.W_NE_prior)
                    this.W_High_prior = this.W_NE_prior;
                if (this.W_High_prior < this.W_SE_prior)
                    this.W_High_prior = this.W_SE_prior;
                if (this.W_High_prior < this.W_N_prior)
                    this.W_High_prior = this.W_N_prior;
            }
        }

        public void check_for_highest_black()  //после каждого изменение приоритета проверка на наивысший приоритет среди 4 направлений для черного игрока
        {
            if (this.piece == 0)
            {
                if(this.B_High_prior<this.B_E_prior)
                this.B_High_prior = this.B_E_prior;
                if (this.B_High_prior < this.B_NE_prior)
                    this.B_High_prior = this.B_NE_prior;
                if (this.B_High_prior < this.B_SE_prior)
                    this.B_High_prior = this.B_SE_prior;
                if (this.B_High_prior < this.B_N_prior)
                    this.B_High_prior = this.B_N_prior;
            }
        }

    }

    class Gomoku_game
    {
        White_Player white_player; 
        Black_Player black_player;
        int turn = 1;
        bool game_over = false;
        static public Condition[,] game_board = new Condition[15, 15];      // игровая доска, которая представляет собой массив объектов, которые описывают приоритеты для каждого игрока


        public Gomoku_game(White_Player white_player, Black_Player black_player)
        {
            this.white_player = white_player;
            this.black_player = black_player;
        }
        public void Game_start()
        {

            for (int i = 0; i < 15; i++)
                for (int j = 0; j < 15; j++)
                {
                    game_board[i, j] = new Condition();
                }
            white_player.first_turn();   // белый и черный игроки делают свои первые ходы
            black_player.turn();

                        while (!game_over)    // игра продолжается до тех пор, пока один из игроков не соберет пять в ряд, то есть на начало его хода на доске будет ячейка с приоритетом 5, либо же когда вся доска будет заполнена, но победителя не будет
                          {
                            if(turn%2!=0)
                            {
                                if (white_player.cur_high_prior >= 5)
                                {
                                    game_over = true;
                                    Console.WriteLine("White won");
                                }else if(white_player.cur_high_prior==0)
                                {
                                    game_over = true;
                                    Console.WriteLine("Tie");
                                }
                                white_player.turn();
                                turn++;


                            }
                            else if(turn%2==0)
                            {
                                if (black_player.cur_high_prior >= 5)
                                {
                                    game_over = true;
                                    Console.WriteLine("Black won");

                                }
                                else if (black_player.cur_high_prior == 0)
                                {
                                    game_over = true;
                                    Console.WriteLine("Tie");
                                }
                                black_player.turn();
                                turn++;


                            }
                          }
                        
        }
    }
    class Gomoku_player
    {

        public Condition[,] game_board = Gomoku_game.game_board;
        public int last_turn_x = 0;
        public int last_turn_y = 0;
        public int cur_high_prior = 0;
        public Gomoku_player()
        {

        }


        public virtual void turn()
        {

        }

    }

    class White_Player : Gomoku_player
    {
        public Black_Player opponent { get; set; }

        public White_Player()
        {

        }
        public void first_turn()        // первый ход для белого игрока, где он ставит свою фигуру на случайную клетку в середине доски, чтобы была возможость идти по любому направлению
        {
            Random rng = new Random();
            int last_x = rng.Next(4,10);
            int last_y = rng.Next(4, 10);
            game_board[last_x, last_y].piece = 1;
            analyze(last_x, last_y);
            game_board[last_x, last_y].B_High_prior = 0;
            game_board[last_x, last_y].W_High_prior = 0;
        }


        public override void turn() 
        {


            int opp_max_prior=0;
            int this_max_prior = 0;

            if (this.cur_high_prior >= opponent.cur_high_prior )        // если приоритет белого игрока выше или равен приоритету черного игрока, то он делает свой ход, ища ячейку с наиболее высоким приоритетом для себя, а затем среди них выбирает ту, в которой приоритет черного игрока наивысший
            {
                for (int i = 0; i < 15; i++)        // цикл, в котором определяется наивысший приоритет оппонента в ячейках с наивысшим приоритетом для белого
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].W_High_prior == cur_high_prior)
                        {
                            if (opp_max_prior < game_board[i, j].B_High_prior)
                                opp_max_prior = game_board[i, j].B_High_prior;
                        }
                    }
                }

                for (int i = 0; i < 15; i++)   //цикл, в котором, среди клеток с наивысшим приоритетом для белого, находятся клетки с наивысшим приоритетом для черного
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].W_High_prior == cur_high_prior && game_board[i, j].B_High_prior == opp_max_prior && game_board[i, j].piece == 0)
                        {
                            last_turn_x = i;
                            last_turn_y = j;

                        }
                    }
                }

            }

            if (this.cur_high_prior <= opponent.cur_high_prior)     // если же приоритет оппонента выше, то белый игрок начинает сначала искать ячейки с наивысшим приоритетом для черного игрока, а затем среди них выбирает наивысшую для себя
            {

                for (int i = 0; i < 15; i++)        //цикл, в котором находится максимальный приоритет для черного
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].B_High_prior == opponent.cur_high_prior)
                        {
                            if (this_max_prior < game_board[i, j].W_High_prior)
                                this_max_prior = game_board[i, j].W_High_prior;
                        }
                    }
                }
                for (int i = 0; i < 15; i++)        //цикл, в котором находится максимальный приоритет для белого, среди найденных клеток
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].B_High_prior == opponent.cur_high_prior && game_board[i, j].W_High_prior == this_max_prior && game_board[i,j].piece==0)
                        {
                            last_turn_x = i;
                            last_turn_y = j;
                        }
                    }

                }
            }


            game_board[last_turn_x, last_turn_y].piece = 1;             // после нахождения ячейки в нее ставится фигура, для обоих игроков приоритет понижается до 0 и проводится анализ по 4 направлениям
            game_board[last_turn_x, last_turn_y].B_High_prior = 0;
            game_board[last_turn_x, last_turn_y].W_High_prior = 0;
            analyzeE(last_turn_x, last_turn_y);
            analyzeNE(last_turn_x, last_turn_y);
            analyzeSE(last_turn_x, last_turn_y);
            analyzeN(last_turn_x, last_turn_y);

            int temp_max_w=0;                       // проверка на изменения максимального приоритета на доске
            int temp_max_b=0;

            for(int i=0;i<15;i++)
            {
                for(int j=0;j<15;j++)
                {
                    if (temp_max_b < game_board[i, j].B_High_prior)
                        temp_max_b = game_board[i, j].B_High_prior;
                    if (temp_max_w < game_board[i, j].W_High_prior)
                        temp_max_w = game_board[i, j].W_High_prior;
                }
            }
            this.cur_high_prior = temp_max_w;
            opponent.cur_high_prior = temp_max_b;
        }

      
        public void analyze(int x,int y)        // функция, которая собирает все 4 функции анализа направлений
        {
            analyzeE(x, y);
            analyzeN(x, y);
            analyzeNE(x, y);
            analyzeSE(x, y);
        }
        public void analyzeNE(int x, int y)         // одна из четырех функций анализа, которая анализирует северо-восточное направление на игровой доске
        {
            int w_counter = 0;  // количество белых фигур на участке
            int b_counter = 0;  // количество черных фигур на участке
            int down_corr = 5;  // корректировка с нижней части отсчета, если координата находится на краю доски и количество участков нужно уменьшить, так как они не помещаются 
            int up_corr =5;

            if ((x > 10 && y < 4) || (x < 4 && y > 10)) // если координата находится в вехнем левом углу или в нижнем правом, то анализ невозможен, поэтому функция работать не будет
                return;

            if (x > 9 || y > 9)     // если координата находится в верней или правой части, то необходимо рассчитать корректировку, насколько меньше операций нужно провести
            {
                if (x > y || x == y)        // если координата находится ближе к верхнему краю, чем к левому, или расстояние одинаково
                    up_corr = 14 - x;
                else if (x < y) up_corr = 14 - y;   // если координата находится ближе к левому краю
            }

            if (x < 5 || y < 5)     //если координата находится в нижней или левой части
            {
                if (x < y || x == y)    //если координата находится ближе к левому краю или расстояние одинаково
                    down_corr = x;  
                else if (x > y) down_corr = y;      //если координата находится ближе к нижнему краю
            }

            for(int i=5-down_corr; i<2+up_corr;i++)     //начало цикла, в котором анализируются участки по 5 ячеек в ряд
            {
                w_counter = 0;      //количество белых на участке
                b_counter = 0;      //количетсво черных на участке
                for(int j=0;j<5;j++)    //цикл анализа участка
                {
                    if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 1)
                        w_counter++;
                    else if(game_board[x - 5 + i + j, y - 5 + i + j].piece == 2)
                        b_counter++;
                }
                if (b_counter == 0) //если на участке нет черных фигур, то присваиваем пустым клеткам приоритет, равный количеству белыъ фигур+1
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 0 && game_board[x - 5 + i + j, y - 5 + i + j].W_NE_prior < w_counter+1) // если приоритет меньше, чем количество фигур+1
                        {   
                            if (this.cur_high_prior <= w_counter+1) //проверка на наивысший приоритет по всей доске
                                this.cur_high_prior = w_counter+1;
                            game_board[x - 5 + i + j, y - 5 + i + j].W_NE_prior = w_counter+1;
                            game_board[x - 5 + i + j, y - 5 + i + j].check_for_highest_white();    //проверка на наивысший приоритет в данной клетке
                            game_board[x - 5 + i + j, y - 5 + i + j].isprotected = true;            //пометка, что клетка была изменена, и недоступна для изменения
                        }
                    }
                }
                if (b_counter != 0)         //если количество черных фигур не равно нулю  
                    for (int j = 0; j < 5; j++)
                        if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 0)
                            game_board[x - 5 + i + j, y - 5 + i + j].set_to_0 = true;   // пометка, что ячейку нужно рассмотреть на понижение приоритета до нуля
            }
            for(int i=5-down_corr;i<6+up_corr;i++)      //проверка всех клеток на понижение до нуля
            {
                if (game_board[x-5+i, y-5+i].set_to_0 && !game_board[x-5+i, y-5+i].isprotected)
                {
                    game_board[x-5+i, y-5+i].B_NE_prior = 0;
                    game_board[x-5+i, y-5+i].check_for_highest_black();
                }
                game_board[x-5+i, y-5+i].set_to_0 = false;      //обнуление переменных
                game_board[x-5+i, y-5+i].isprotected = false;
            }

        }    

        public void analyzeE(int x,int y)       //все остальные функции аналогичны для обоих игроков, различаются лишь координаты
        {
            int w_counter = 0;
            int b_counter = 0;
            int left_corr = 5;
            int right_corr = 5;
            if (x < 5)
                left_corr = x;
            if (x > 9)
                right_corr = 14 - x;
            for (int i = 5 - left_corr; i < 2 + right_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x - 5 + i + j, y].piece == 1)
                        w_counter++;
                    else if (game_board[x - 5 + i + j, y].piece == 2)
                        b_counter++;
                }
                if (b_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x - 5 + i + j, y].piece == 0 && game_board[x - 5 + i + j, y].W_E_prior < w_counter+1)
                        {
                            if (this.cur_high_prior <= w_counter+1)
                                this.cur_high_prior = w_counter+1;
                            game_board[x - 5 + i + j, y].W_E_prior = w_counter+1;
                            game_board[x - 5 + i + j, y].check_for_highest_white();
                            game_board[x - 5 + i + j, y].isprotected = true;
                        }
                    }
                }
                if (b_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x - 5 + i + j, y].piece == 0)
                            game_board[x - 5 + i + j, y].set_to_0 = true;
            }
            for (int i = 5 - left_corr; i < 6 + right_corr; i++)
            {
                if (game_board[x - 5 + i, y].set_to_0 && !game_board[x - 5 + i, y].isprotected)
                {
                    game_board[x - 5 + i, y].B_E_prior = 0;
                    game_board[x - 5 + i, y].check_for_highest_black();
                }
                game_board[x - 5 + i, y].set_to_0 = false;
                game_board[x -5 +i, y].isprotected = false;
            }


        } //problem

        public void analyzeSE(int x, int y)
        {
            int w_counter = 0;
            int b_counter = 0;
            int down_corr = 5;
            int up_corr = 5;

            if ((x > 10 && y > 10) || (x < 4 && y < 4))  // up and down corr are messed
                return;

            if (x < 5 || y > 9)
            {
                if (x < 5 && y > 9)
                {
                    if (x <= 14-y )
                        down_corr = x;
                    else  down_corr = 14 - y;
                }else if(x<5)
                {
                    down_corr = x;
                }else if(y>9)
                {
                    down_corr = 14 - y;
                }    
            }

            if (x > 9 || y < 5)
            {
                if (x > 9 && y < 5)
                {
                    if (y <=14-x)
                        up_corr = y;
                    else  up_corr = 14-x;
                }else if(x>9)
                {
                    up_corr = 14 - x;
                }else if(y<5)
                {
                    up_corr = y;
                }
            }
            for (int i = 5 - down_corr; i < 2 + up_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 1)
                        w_counter++;
                    else if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 2)
                        b_counter++;
                }
                if (b_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 0 && game_board[x - 5 + i + j, y + 5 - i - j].W_SE_prior < w_counter+1)
                        {
                            if (this.cur_high_prior <= w_counter+1)
                                this.cur_high_prior = w_counter+1;
                            game_board[x - 5 + i + j, y + 5 - i - j].W_SE_prior = w_counter+1;
                            game_board[x - 5 + i + j, y + 5 - i - j].check_for_highest_white();
                            game_board[x - 5 + i + j, y + 5 - i - j].isprotected = true;
                        }
                    }
                }
                if (b_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 0)
                            game_board[x - 5 + i + j, y + 5 - i - j].set_to_0 = true;
            }
            for (int i = 5 - down_corr; i < 6 + up_corr; i++)
            {
                if (game_board[x - 5 + i, y + 5 - i].set_to_0 && !game_board[x - 5 + i, y + 5 - i].isprotected)
                {
                    game_board[x - 5 + i, y + 5 - i].B_SE_prior = 0;
                    game_board[x - 5 + i, y + 5 - i].check_for_highest_black();
                }
                game_board[x - 5 + i, y + 5 - i].set_to_0 = false;
                game_board[x -5 +i, y + 5 - i].isprotected = false;
            }


        }

        public void analyzeN(int x,int y)
        {
            int w_counter = 0;
            int b_counter = 0;
            int up_corr = 5;
            int down_corr = 5;
            if (y < 5)
                down_corr = y;
            if (y > 9)
                up_corr = 14 - y;
            for (int i = 5 - down_corr; i < 2 + up_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x , y - 5 + i + j].piece == 1)
                        w_counter++;
                    else if (game_board[x , y - 5 + i + j].piece == 2)
                        b_counter++;
                }
                if (b_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x , y - 5 + i + j].piece == 0 && game_board[x , y - 5 + i + j].W_N_prior < w_counter+1)
                        {
                            if (this.cur_high_prior <= w_counter+1)
                                this.cur_high_prior = w_counter+1;
                            game_board[x , y - 5 + i + j].W_N_prior = w_counter+1;
                            game_board[x , y - 5 + i + j].check_for_highest_white();
                            game_board[x , y - 5 + i + j].isprotected = true;
                        }
                    }
                }
                if (b_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x , y - 5 + i + j].piece == 0)
                            game_board[x , y - 5 + i + j].set_to_0 = true;
            }
            for (int i = 5 - down_corr; i < 6 + up_corr; i++)
            {
                if (game_board[x , y - 5 + i].set_to_0 && !game_board[x, y - 5 + i].isprotected)
                {
                    game_board[x , y - 5 + i].B_N_prior = 0;
                    game_board[x , y - 5 + i].check_for_highest_black();
                }
                game_board[x , y - 5 + i].set_to_0 = false;
                game_board[x , y - 5 + i].isprotected = false;
            }

        }
    }
    class Black_Player : Gomoku_player
    {
        public White_Player opponent { get; set; }      //черный игрок идентичен белому, лишь за исключением, что в функции анализа черные фигуры заменены белыми и наоборот, а также более жесткое ограничение на возможность хода

        public Black_Player()
        {

        }



        public void analyze(int x, int y)
        {
            analyzeE(x, y);
            analyzeN(x, y);
            analyzeNE(x, y);
            analyzeSE(x, y);
        }



        public override void turn() 
        {

           
            int opp_max_prior = 0;
            int this_max_prior = 0;

            if (this.cur_high_prior > opponent.cur_high_prior||cur_high_prior==5)
            {
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].B_High_prior == cur_high_prior)
                        {
                            if (opp_max_prior < game_board[i, j].W_High_prior)
                                opp_max_prior = game_board[i, j].W_High_prior;
                        }
                    }
                }

                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].B_High_prior == cur_high_prior&&game_board[i,j].W_High_prior==opp_max_prior)
                        {
                            last_turn_x = i;
                            last_turn_y = j;

                        }
                    }
                }
                
            }

            if (this.cur_high_prior <= opponent.cur_high_prior)
            {

                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].W_High_prior == opponent.cur_high_prior)
                        {
                            if (this_max_prior < game_board[i, j].B_High_prior)
                                this_max_prior = game_board[i, j].B_High_prior;
                        }
                    }
                }
                for (int i = 0; i < 15; i++)
                {
                    for (int j = 0; j < 15; j++)
                    {
                        if (game_board[i, j].W_High_prior == opponent.cur_high_prior && game_board[i, j].B_High_prior == this_max_prior)
                        {
                            last_turn_x = i;
                            last_turn_y = j;
                        }
                    }

                }
            
        }

            game_board[last_turn_x, last_turn_y].piece = 2;
            game_board[last_turn_x, last_turn_y].B_High_prior = 0;
            game_board[last_turn_x, last_turn_y].W_High_prior = 0;
            analyzeE(last_turn_x, last_turn_y);
            analyzeNE(last_turn_x, last_turn_y);
            analyzeSE(last_turn_x, last_turn_y);
            analyzeN(last_turn_x, last_turn_y);

            int temp_max_w = 0;
            int temp_max_b = 0;

            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    if (temp_max_b < game_board[i, j].B_High_prior)
                        temp_max_b = game_board[i, j].B_High_prior;
                    if (temp_max_w < game_board[i, j].W_High_prior)
                        temp_max_w = game_board[i, j].W_High_prior;
                }
            }
            this.cur_high_prior = temp_max_b;
            opponent.cur_high_prior = temp_max_w;

        }
        public void analyzeNE(int x, int y)
        {
            int w_counter = 0;
            int b_counter = 0;
            int down_corr = 5;
            int up_corr = 5;

            if ((x > 10 && y < 4) || (x < 4 && y > 10))
                return;

            if (x > 9 || y > 9)
            {
                if (x > y || x == y)
                    up_corr = 14 - x;
                else if (x < y) up_corr = 14 - y;
            }

            if (x < 5 || y < 5)
            {
                if (x < y || x == y)
                    down_corr = x;
                else if (x > y) down_corr = y;
            }

            for (int i = 5 - down_corr; i < 2 + up_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 1)
                        w_counter++;
                    else if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 2)
                        b_counter++;
                }
                if (w_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 0 && game_board[x - 5 + i + j, y - 5 + i + j].B_NE_prior < b_counter + 1)
                        {
                            if (this.cur_high_prior <= b_counter + 1)
                                this.cur_high_prior = b_counter + 1;
                            game_board[x - 5 + i + j, y - 5 + i + j].B_NE_prior = b_counter + 1;
                            game_board[x - 5 + i + j, y - 5 + i + j].check_for_highest_black();
                            game_board[x - 5 + i + j, y - 5 + i + j].isprotected = true;
                        }
                    }
                }
                if (w_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x - 5 + i + j, y - 5 + i + j].piece == 0)
                            game_board[x - 5 + i + j, y - 5 + i + j].set_to_0 = true;
            }
            for (int i = 5 - down_corr; i < 6 + up_corr; i++)
            {
                if (game_board[x - 5 + i, y - 5 + i].set_to_0 && !game_board[x - 5 + i, y - 5 + i].isprotected)
                {
                    game_board[x - 5 + i, y - 5 + i].W_NE_prior = 0;
                    game_board[x - 5 + i, y - 5 + i].check_for_highest_white();
                }
                game_board[x - 5 + i, y - 5 + i].set_to_0 = false;
                game_board[x - 5 + i, y - 5 + i].isprotected = false;
            }

        }
        public void analyzeE(int x, int y)
        {
            int w_counter = 0;
            int b_counter = 0;
            int left_corr = 5;
            int right_corr = 5;
            if (x < 5)
                left_corr = x;
            if (x > 9)
                right_corr = 14 - x;
            for (int i = 5 - left_corr; i < 2 + right_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x - 5 + i + j, y].piece == 1)
                        w_counter++;
                    else if (game_board[x - 5 + i + j, y].piece == 2)
                        b_counter++;
                }
                if (w_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x - 5 + i + j, y].piece == 0 && game_board[x - 5 + i + j, y].B_E_prior < b_counter + 1)
                        {
                            if (this.cur_high_prior <= b_counter + 1)
                                this.cur_high_prior = b_counter + 1;
                            game_board[x - 5 + i + j, y].B_E_prior = b_counter + 1;
                            game_board[x - 5 + i + j, y].check_for_highest_black();
                            game_board[x - 5 + i + j, y].isprotected = true;
                        }
                    }
                }
                if (w_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x - 5 + i + j, y].piece == 0)
                            game_board[x - 5 + i + j, y].set_to_0 = true;
            }
            for (int i = 5 - left_corr; i < 6 + right_corr; i++)
            {
                if (game_board[x - 5 + i, y].set_to_0 && !game_board[x - 5 + i, y].isprotected)
                {
                    game_board[x - 5 + i, y].W_E_prior = 0;
                    game_board[x - 5 + i, y].check_for_highest_white();
                }
                game_board[x - 5 + i, y].set_to_0 = false;
                game_board[x - 5 + i, y].isprotected = false;
            }


        }
        public void analyzeSE(int x, int y)
        {
            int w_counter = 0;
            int b_counter = 0;
            int down_corr = 5;
            int up_corr = 5;

            if ((x > 10 && y > 10) || (x < 4 && y < 4)) 
                return;

            if (x < 5 || y > 9)
            {
                if (x < 5 && y > 9)
                {
                    if (x <= 14 - y)
                        down_corr = x;
                    else down_corr = 14 - y;
                }
                else if (x < 5)
                {
                    down_corr = x;
                }
                else if (y > 9)
                {
                    down_corr = 14 - y;
                }
            }

            if (x > 9 || y < 5)
            {
                if (x > 9 && y < 5)
                {
                    if (y <= 14 - x)
                        up_corr = y;
                    else up_corr = 14 - x;
                }
                else if (x > 9)
                {
                    up_corr = 14 - x;
                }
                else if (y < 5)
                {
                    up_corr = y;
                }
            }
            for (int i = 5 - down_corr; i < 2 + up_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 1)
                        w_counter++;
                    else if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 2)
                        b_counter++;
                }
                if (w_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 0 && game_board[x - 5 + i + j, y + 5 - i - j].B_SE_prior < b_counter + 1)
                        {
                            if (this.cur_high_prior <= b_counter + 1)
                                this.cur_high_prior = b_counter + 1;
                            game_board[x - 5 + i + j, y + 5 - i - j].B_SE_prior = b_counter + 1;
                            game_board[x - 5 + i + j, y + 5 - i - j].check_for_highest_black();
                            game_board[x - 5 + i + j, y + 5 - i - j].isprotected = true;
                        }
                    }
                }
                if (w_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x - 5 + i + j, y + 5 - i - j].piece == 0)
                            game_board[x - 5 + i + j, y + 5 - i - j].set_to_0 = true;
            }
            for (int i = 5 - down_corr; i < 6 + up_corr; i++)
            {
                if (game_board[x - 5 + i, y + 5 - i].set_to_0 && !game_board[x - 5 + i, y + 5 - i].isprotected)
                {
                    game_board[x - 5 + i, y + 5 - i].W_SE_prior = 0;
                    game_board[x - 5 + i, y + 5 - i].check_for_highest_white();
                }
                game_board[x - 5 + i, y + 5 - i].set_to_0 = false;
                game_board[x - 5 + i, y + 5 - i].isprotected = false;
            }


        }
        public void analyzeN(int x, int y)
        {
            int w_counter = 0;
            int b_counter = 0;
            int up_corr = 5;
            int down_corr = 5;
            if (y < 5)
                down_corr = y;
            if (y > 9)
                up_corr = 14 - y;
            for (int i = 5 - down_corr; i < 2 + up_corr; i++)
            {
                w_counter = 0;
                b_counter = 0;
                for (int j = 0; j < 5; j++)
                {
                    if (game_board[x, y - 5 + i + j].piece == 1)
                        w_counter++;
                    else if (game_board[x, y - 5 + i + j].piece == 2)
                        b_counter++;
                }
                if (w_counter == 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (game_board[x, y - 5 + i + j].piece == 0 && game_board[x, y - 5 + i + j].B_N_prior < b_counter + 1)
                        {
                            if (this.cur_high_prior <= b_counter + 1)
                                this.cur_high_prior = b_counter + 1;
                            game_board[x, y - 5 + i + j].B_N_prior = b_counter + 1;
                            game_board[x, y - 5 + i + j].check_for_highest_black();
                            game_board[x, y - 5 + i + j].isprotected = true;
                        }
                    }
                }
                if (b_counter != 0)
                    for (int j = 0; j < 5; j++)
                        if (game_board[x, y - 5 + i + j].piece == 0)
                            game_board[x, y - 5 + i + j].set_to_0 = true;
            }
            for (int i = 5 - down_corr; i < 6 + up_corr; i++)
            {
                if (game_board[x, y - 5 + i].set_to_0 && !game_board[x, y - 5 + i].isprotected)
                {
                    game_board[x, y - 5 + i].W_N_prior = 0;
                    game_board[x, y - 5 + i].check_for_highest_white();
                }
                game_board[x, y - 5 + i].set_to_0 = false;
                game_board[x, y - 5 + i].isprotected = false;
            }

        }
    }
    }
    



    class Program
    {
        static void Main(string[] args)
        {
        Gomoku.White_Player white_Player = new Gomoku.White_Player();
        Gomoku.Black_Player black_Player = new Gomoku.Black_Player();
        white_Player.opponent = black_Player;
        black_Player.opponent = white_Player;
        Gomoku.Gomoku_game game = new Gomoku.Gomoku_game(white_Player, black_Player);
        game.Game_start();
        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
            {
                if (Gomoku.Gomoku_game.game_board[j, i].piece == 1)
                    Console.Write("x");
                else if (Gomoku.Gomoku_game.game_board[j, i].piece == 2)
                    Console.Write("o");
                else
                    Console.Write("_");


            }
            Console.WriteLine();
        }
        Console.ReadKey();
    }
    }


