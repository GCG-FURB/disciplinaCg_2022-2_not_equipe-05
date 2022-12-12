#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
  class Mundo : GameWindow
  {
    private static Mundo instanciaMundo = null;

    private Mundo(int width, int height) : base(width, height) { }

    public static Mundo GetInstance(int width, int height)
    {
      if (instanciaMundo == null)
        instanciaMundo = new Mundo(width, height);
      return instanciaMundo;
    }

    private CameraPerspective camera = new CameraPerspective();
    protected List<Objeto> objetosLista = new List<Objeto>();
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private String menuSelecao = "";
    private char menuEixoSelecao = 'z';
    private float deslocamento = 2;
    private bool bBoxDesenhar = false;
    private Random random = new Random();
    private Cubo tabuleiro;
    private Cubo pacman;
    private Cubo ponto;
    private Cubo parede;
    private List<Cubo> fantasmas = new List<Cubo>();
    private int posicaoX = 1;
    private int posicaoY = 1;
    private int qntComida = 0;
    private int vida = 3;
    //1 = Parede
    //2 = Pacman
    //3 = pontos
    int[][] mapa = new[]
        {
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
            new[] { 1, 2, 0, 0, 0, 0, 0, 0, 0, 3, 0, 3, 3, 0, 1 },
            new[] { 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1 },
            new[] { 1, 0, 1, 0, 0, 0, 3, 3, 3, 0, 1, 0, 1, 0, 1 },
            new[] { 1, 3, 1, 0, 1, 0, 1, 1, 1, 1, 1, 0, 1, 3, 1 },
            new[] { 1, 3, 1, 3, 1, 0, 3, 3, 3, 0, 1, 0, 1, 3, 1 },
            new[] { 1, 3, 0, 3, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1 },
            new[] { 1, 3, 1, 3, 1, 3, 1, 0, 0, 4, 1, 0, 1, 3, 1 },
            new[] { 1, 3, 1, 0, 1, 3, 1, 4, 0, 0, 1, 0, 1, 3, 1 },
            new[] { 1, 0, 1, 0, 0, 3, 1, 0, 0, 4, 1, 0, 0, 0, 1 },
            new[] { 1, 0, 1, 0, 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1 },
            new[] { 1, 3, 1, 0, 0, 0, 0, 0, 0, 3, 0, 0, 1, 0, 1 },
            new[] { 1, 3, 1, 0, 0, 1, 1, 1, 1, 3, 1, 1, 1, 0, 1 },
            new[] { 1, 3, 3, 3, 0, 3, 3, 3, 3, 0, 0, 0, 0, 0, 1 },
            new[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
        };

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");
      RestaurarCamera();
      
      this.desenhaTabuleiro();

      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.CullFace);
    }

    private void RestaurarCamera()
    {
            Vector3 padrao = Vector3.Zero;
            padrao.Y = 15;
            padrao.Z = 25;
            padrao.X = 17;
            camera.Eye = padrao;
    }

    /*private ObjetoGeometria adicionarTabuleiro(int largura, int profundidade){
      return tabuleiro;
    }*/
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);

      GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

      Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(camera.Fovy, Width / (float)Height, camera.Near, camera.Far);
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadMatrix(ref projection);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      Matrix4 modelview = Matrix4.LookAt(camera.Eye, camera.At, camera.Up);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadMatrix(ref modelview);
#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();
      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if(this.qntComida != 0 && vida != 0) 
      {
        if (e.Key == Key.S)
        {
          int proximaPosicao = mapa[posicaoX][posicaoY + 1];
          if (proximaPosicao == 4) {
            vida--;
            mapa[posicaoX][posicaoY] = 0;
            if (mapa[1][1] != 4) {
              mapa[1][1] = 2;
              posicaoX = 1;
              posicaoY = 1;
            } 
            else if (mapa[1][13] != 4) {
              mapa[1][13] = 2;
              posicaoX = 1;
              posicaoY = 13;
            }
            else if (mapa[13][1] != 4) {
              mapa[13][1] = 2;
              posicaoX = 13;
              posicaoY = 1;
            }
            else if (mapa[13][13] != 4) {
              mapa[13][13] = 2;
              posicaoX = 13;
              posicaoY = 13;
            }
          }
          else if (proximaPosicao != 1) {
            mapa[posicaoX][posicaoY + 1] = 2;
            mapa[posicaoX][posicaoY] = 0;
            posicaoY++;
            if (proximaPosicao == 3) {
              qntComida--;
            }
          } 
          this.moveFantasma();
          this.desenhaTabuleiro();
        }
        else if (e.Key == Key.W)
        {
          int proximaPosicao = mapa[posicaoX][posicaoY - 1];
          if (proximaPosicao == 4) {
            vida--;
            mapa[posicaoX][posicaoY] = 0;
            if (mapa[1][1] != 4) {
              mapa[1][1] = 2;
              posicaoX = 1;
              posicaoY = 1;
            } 
            else if (mapa[1][13] != 4) {
              mapa[1][13] = 2;
              posicaoX = 1;
              posicaoY = 13;
            }
            else if (mapa[13][1] != 4) {
              mapa[13][1] = 2;
              posicaoX = 13;
              posicaoY = 1;
            }
            else if (mapa[13][13] != 4) {
              mapa[13][13] = 2;
              posicaoX = 13;
              posicaoY = 13;
            }
          }
          else if(proximaPosicao != 1) {
            mapa[posicaoX][posicaoY - 1] = 2;
            mapa[posicaoX][posicaoY] = 0;
            posicaoY--;
            if (proximaPosicao == 3) {
              qntComida--;
            }
          }
          this.moveFantasma();
          this.desenhaTabuleiro();
        }
        else if (e.Key == Key.D)
        {
          int proximaPosicao = mapa[posicaoX + 1][posicaoY];
          if (proximaPosicao == 4) {
            vida--;
            mapa[posicaoX][posicaoY] = 0;
            if (mapa[1][1] != 4) {
              mapa[1][1] = 2;
              posicaoX = 1;
              posicaoY = 1;
            } 
            else if (mapa[1][13] != 4) {
              mapa[1][13] = 2;
              posicaoX = 1;
              posicaoY = 13;
            }
            else if (mapa[13][1] != 4) {
              mapa[13][1] = 2;
              posicaoX = 13;
              posicaoY = 1;
            }
            else if (mapa[13][13] != 4) {
              mapa[13][13] = 2;
              posicaoX = 13;
              posicaoY = 13;
            }
          }
          else if(proximaPosicao != 1) {
            mapa[posicaoX + 1][posicaoY] = 2;
            mapa[posicaoX][posicaoY] = 0;
            posicaoX++;
            if (proximaPosicao == 3) {
              qntComida--;
            }
          }
          this.moveFantasma();
          this.desenhaTabuleiro();
        }
        else if (e.Key == Key.A)
        {
          int proximaPosicao = mapa[posicaoX-1][posicaoY];
          if (proximaPosicao == 4) {
            vida--;
            mapa[posicaoX][posicaoY] = 0;
            if (mapa[1][1] != 4) {
              mapa[1][1] = 2;
              posicaoX = 1;
              posicaoY = 1;
            } 
            else if (mapa[1][13] != 4) {
              mapa[1][13] = 2;
              posicaoX = 1;
              posicaoY = 13;
            }
            else if (mapa[13][1] != 4) {
              mapa[13][1] = 2;
              posicaoX = 13;
              posicaoY = 1;
            }
            else if (mapa[13][13] != 4) {
              mapa[13][13] = 2;
              posicaoX = 13;
              posicaoY = 13;
            }
          }
          else if(proximaPosicao != 1) {
            mapa[posicaoX-1][posicaoY] = 2;
            mapa[posicaoX][posicaoY] = 0;
            posicaoX--;
            if (proximaPosicao == 3) {
              qntComida--;
            }
          }
          this.moveFantasma();
          this.desenhaTabuleiro();
        }
      } else if (qntComida == 0) 
      {
        Console.WriteLine("Você venceu!");
      } else if(vida == 0) {
        Console.WriteLine("Você perdeu!");
      }
      // Console.Clear(); //TODO: não funciona.
      if (e.Key == Key.H) Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape) Exit();
      //--------------------------------------------------------------
      else if (e.Key == Key.Number9)
        objetoSelecionado = null;                     // desmacar objeto selecionado
      else if (e.Key == Key.B)
        bBoxDesenhar = !bBoxDesenhar;     //FIXME: bBox não está sendo atualizada.
      else if (e.Key == Key.E)
      {
        Console.WriteLine("--- Objetos / Pontos: ");
        for (var i = 0; i < objetosLista.Count; i++)
        {
          Console.WriteLine(objetosLista[i]);
        }
      }
      //--------------------------------------------------------------
      else if (e.Key == Key.X) menuEixoSelecao = 'x';
      else if (e.Key == Key.Y) menuEixoSelecao = 'y';
      else if (e.Key == Key.Z) menuEixoSelecao = 'z';
      else if (e.Key == Key.Minus) deslocamento--;
      else if (e.Key == Key.Plus) deslocamento++;
      else if (e.Key == Key.C) menuSelecao = "[menu] C: Câmera";
      else if (e.Key == Key.O) menuSelecao = "[menu] O: Objeto";

      // Menu: seleção
      else if (menuSelecao.Equals("[menu] C: Câmera")) camera.MenuTecla(e.Key, menuEixoSelecao, deslocamento);
      else if (menuSelecao.Equals("[menu] O: Objeto"))
      {
        if (objetoSelecionado != null) objetoSelecionado.MenuTecla(e.Key, menuEixoSelecao, deslocamento, bBoxDesenhar);
        else Console.WriteLine(" ... Objeto NÃO selecionado.");
      }

      else
        Console.WriteLine(" __ Tecla não implementada.");

      if (!(e.Key == Key.LShift)) //FIXME: não funciona.
        Console.WriteLine("__ " + menuSelecao + "[" + deslocamento + "]");

    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
    }

    private void moveFantasma() {
      foreach (Cubo fantasma in fantasmas)
      {
        int numeroRandom = random.Next(1, 10000);
        if (numeroRandom <= 5000) {
          if (numeroRandom <= 2500) {
            int proximaPosicao = mapa[fantasma.posicaoX + 1][fantasma.posicaoY];
            if (proximaPosicao == 2) 
            {
              vida--;
              if (mapa[1][1] != 4) {
                mapa[1][1] = 2;
                posicaoX = 1;
                posicaoY = 1;
              } 
              else if (mapa[1][13] != 4) {
                mapa[1][13] = 2;
                posicaoX = 1;
                posicaoY = 13;
              }
              else if (mapa[13][1] != 4) {
                mapa[13][1] = 2;
                posicaoX = 13;
                posicaoY = 1;
              }
              else if (mapa[13][13] != 4) {
                mapa[13][13] = 2;
                posicaoX = 13;
                posicaoY = 13;
              }
            }
            if (proximaPosicao != 1 && proximaPosicao != 4) 
            {
              mapa[fantasma.posicaoX][fantasma.posicaoY] = 0;
              mapa[fantasma.posicaoX][fantasma.posicaoY] = 4;
              fantasma.posicaoX++;
            }
          } else {
            int proximaPosicao = mapa[fantasma.posicaoX - 1][fantasma.posicaoY];
            if (proximaPosicao == 2) 
            {
              vida--;
              if (mapa[1][1] != 4) {
                mapa[1][1] = 2;
                posicaoX = 1;
                posicaoY = 1;
              } 
              else if (mapa[1][13] != 4) {
                mapa[1][13] = 2;
                posicaoX = 1;
                posicaoY = 13;
              }
              else if (mapa[13][1] != 4) {
                mapa[13][1] = 2;
                posicaoX = 13;
                posicaoY = 1;
              }
              else if (mapa[13][13] != 4) {
                mapa[13][13] = 2;
                posicaoX = 13;
                posicaoY = 13;
              }
            }
            if (proximaPosicao != 1 && proximaPosicao != 4) {
              mapa[fantasma.posicaoX][fantasma.posicaoY] = 0;
              mapa[fantasma.posicaoX - 1][fantasma.posicaoY] = 4;
              fantasma.posicaoX--;
            }
          }
        } else {
          if (numeroRandom <= 7500) {
            int proximaPosicao = mapa[fantasma.posicaoX][fantasma.posicaoY + 1];
            if (proximaPosicao == 2) 
            {
              vida--;
              if (mapa[1][1] != 4) {
                mapa[1][1] = 2;
                posicaoX = 1;
                posicaoY = 1;
              } 
              else if (mapa[1][13] != 4) {
                mapa[1][13] = 2;
                posicaoX = 1;
                posicaoY = 13;
              }
              else if (mapa[13][1] != 4) {
                mapa[13][1] = 2;
                posicaoX = 13;
                posicaoY = 1;
              }
              else if (mapa[13][13] != 4) {
                mapa[13][13] = 2;
                posicaoX = 13;
                posicaoY = 13;
              }
            }
            if (proximaPosicao != 1 && proximaPosicao != 4) {
              mapa[fantasma.posicaoX][fantasma.posicaoY] = 0;
              mapa[fantasma.posicaoX][fantasma.posicaoY + 1] = 4;
              fantasma.posicaoY++;
            }
          } else {
            int proximaPosicao = mapa[fantasma.posicaoX][fantasma.posicaoY - 1];
            if (proximaPosicao == 2) 
            {
              vida--;
              if (mapa[1][1] != 4) {
                mapa[1][1] = 2;
                posicaoX = 1;
                posicaoY = 1;
              } 
              else if (mapa[1][13] != 4) {
                mapa[1][13] = 2;
                posicaoX = 1;
                posicaoY = 13;
              }
              else if (mapa[13][1] != 4) {
                mapa[13][1] = 2;
                posicaoX = 13;
                posicaoY = 1;
              }
              else if (mapa[13][13] != 4) {
                mapa[13][13] = 2;
                posicaoX = 13;
                posicaoY = 13;
              }
            }
            if (proximaPosicao != 1 && proximaPosicao != 4) {
              mapa[fantasma.posicaoX][fantasma.posicaoY] = 0;
              mapa[fantasma.posicaoX][fantasma.posicaoY - 1] = 4;
              fantasma.posicaoY--;
            }
          }
        }
      }
    }

    private void desenhaTabuleiro() {
      objetosLista = new List<Objeto>();

      objetoId = Utilitario.charProximo(objetoId);
      tabuleiro = new Cubo(objetoId, null);
      tabuleiro.ObjetoCor.CorR = 2; tabuleiro.ObjetoCor.CorG = 44; tabuleiro.ObjetoCor.CorB = 120;
      tabuleiro.EscalaXYZBBox(15,1,15);
      tabuleiro.Translacao(7.5,'x');
      tabuleiro.Translacao(-0.5,'y');
      tabuleiro.Translacao(7.5,'z');
      objetosLista.Add(tabuleiro);
      int transX = 0;
      int transZ = 0;
      qntComida = 0;
      fantasmas = new List<Cubo>();
      for (int i = 0; i < mapa.Length; i++)
      {
        for (int j = 0; j < mapa[i].Length; j++)
        {
            if (mapa[i][j] == 1)
            {
                objetoId = Utilitario.charProximo(objetoId);
                parede = new Cubo(objetoId, null);
                parede.Translacao(transX+0.5,'x');
                parede.Translacao(transZ+0.5,'z');
                parede.ObjetoCor.CorR = 0;parede.ObjetoCor.CorG = 0;parede.ObjetoCor.CorB = 0;
                objetoSelecionado = parede;
                objetosLista.Add(parede);
            }
            if (mapa[i][j] == 2)
            {
                objetoId = Utilitario.charProximo(objetoId);
                pacman = new Cubo(objetoId, null);
                pacman.ObjetoCor.CorR = 255; pacman.ObjetoCor.CorG = 255; pacman.ObjetoCor.CorB = 0;
                pacman.Translacao(transX+0.5,'x');
                pacman.Translacao(transZ+0.5,'z');
                objetoSelecionado = pacman;
                objetosLista.Add(pacman);
            }
            if (mapa[i][j] == 3)
            {
                objetoId = Utilitario.charProximo(objetoId);
                ponto = new Cubo(objetoId, null);
                ponto.ObjetoCor.CorR = 255; ponto.ObjetoCor.CorG = 130; ponto.ObjetoCor.CorB = 130;
                ponto.Translacao(transX+0.5,'x');
                ponto.Translacao(transZ+0.5,'z');
                objetoSelecionado = ponto;
                objetosLista.Add(ponto);
                qntComida++;
            }
            if (mapa[i][j] == 4)
            {
                objetoId = Utilitario.charProximo(objetoId);
                Cubo fantasma = new Cubo(objetoId, null);
                fantasma.ObjetoCor.CorR = 255; fantasma.ObjetoCor.CorG = 255; fantasma.ObjetoCor.CorB = 255;
                fantasma.Translacao(transX+0.5,'x');
                fantasma.Translacao(transZ+0.5,'z');
                fantasma.posicaoX = i;
                fantasma.posicaoY = j;
                objetoSelecionado = fantasma;
                objetosLista.Add(fantasma);
                fantasmas.Add(fantasma);
            }
            transZ += 1;
        }
        transZ = 0;
        transX += 1;
      }
      objetoSelecionado = pacman;
    }

#if CG_Gizmo
    private void Sru3D()
    {
      //GL.LineWidth(1);
      //GL.Begin(PrimitiveType.Lines);
      // GL.Color3(1.0f,0.0f,0.0f);
      //GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
      //GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      // GL.Color3(0.0f,1.0f,0.0f);
      //GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
      //GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      // GL.Color3(0.0f,0.0f,1.0f);
      //GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
      //GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      //GL.End();
    }
#endif    
  }
  class Program
  {
    static void Main(string[] args)
    {
      ToolkitOptions.Default.EnableHighResolution = false;
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N4";
      window.Run(1.0 / 60.0);
    }
  }
}
