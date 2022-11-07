/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

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

    private CameraOrtho camera = new CameraOrtho();
    protected List<Objeto> objetosLista = new List<Objeto>();
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private bool bBoxDesenhar = false;
    int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
    private bool mouseMoverPto = false;
    private Retangulo obj_Retangulo;
    private Ciurculo obj_Circulo_Maior;
    private Ciurculo obj_Circulo_Menor;
    private Ponto4D ptoCentral = new Ponto4D(200, 200, 0);
    private Privado_BBox obj_BBox;
    private SegReta obj_SegReta;
    private int obj_circulo_raio = 30;
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = 0; camera.xmax = 600; camera.ymin = 0; camera.ymax = 600;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo_Maior = new Ciurculo(objetoId, null, new Ponto4D(200, 200, 0), 150, 90);
      obj_Circulo_Maior.ObjetoCor.CorR = 0; obj_Circulo_Maior.ObjetoCor.CorG = 0; obj_Circulo_Maior.ObjetoCor.CorB =0;
      obj_Circulo_Maior.PrimitivaTipo = PrimitiveType.Lines;
      obj_Circulo_Maior.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo_Maior);
      objetoSelecionado = obj_Circulo_Maior;

      objetoId = Utilitario.charProximo(objetoId);
      Ponto4D ptoAngulo45 = obj_Circulo_Maior.pegaPontosCirculoPeloAngulo(45);
      Ponto4D ptoAngulo225 = obj_Circulo_Maior.pegaPontosCirculoPeloAngulo(225);
      obj_Retangulo = new Retangulo(objetoId, null, ptoAngulo45, ptoAngulo225);
      obj_Retangulo.ObjetoCor.CorR = 255; obj_Retangulo.ObjetoCor.CorG = 0; obj_Retangulo.ObjetoCor.CorB = 255;
      obj_Retangulo.PrimitivaTamanho = 3;
      objetosLista.Add(obj_Retangulo);
      objetoSelecionado = obj_Retangulo;
      obj_BBox = new Privado_BBox(ptoAngulo225.X, ptoAngulo225.Y, ptoAngulo225.Z, ptoAngulo45.X, ptoAngulo45.Y, ptoAngulo45.Z);

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo_Menor = new Ciurculo(objetoId, null, ptoCentral, 50, 36, true);
      obj_Circulo_Menor.ObjetoCor.CorR = 0; obj_Circulo_Menor.ObjetoCor.CorG = 0; obj_Circulo_Menor.ObjetoCor.CorB =0;
      obj_Circulo_Menor.PrimitivaTipo = PrimitiveType.LineStrip;
      obj_Circulo_Menor.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo_Menor);
      objetoSelecionado = obj_Retangulo;

#if CG_Privado
#endif
#if CG_OpenGL
      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
#endif
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
#if CG_OpenGL
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
#endif
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
#if CG_OpenGL
      GL.Clear(ClearBufferMask.ColorBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
#endif
#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();
#if CG_Gizmo
      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
#endif
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.H)
        Utilitario.AjudaTeclado();
      else if (e.Key == Key.Escape)
        Exit();
      else if (e.Key == Key.E)
      {
        Console.WriteLine("--- Objetos / Pontos: ");
        for (var i = 0; i < objetosLista.Count; i++)
        {
          Console.WriteLine(objetosLista[i]);
        }
      }
      else if (e.Key == Key.A) {
        this.ptoCentral.X -= 1;
        if (obj_BBox.validaDentro(this.ptoCentral)) {
          desenhaCirculoMenor(255, 0, 255);
        } else if (obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) > 149 && obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) <= 150) {
          desenhaCirculoMenor(118, 249, 251);
        } else if(obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) < 150) {
          desenhaCirculoMenor(255, 255, 0);
        }
        else {
          this.ptoCentral.X += 1;
        }
      } 
      else if (e.Key == Key.W) {
        this.ptoCentral.Y += 1;
        if (obj_BBox.validaDentro(this.ptoCentral)) {
          desenhaCirculoMenor(255, 0, 255);
        } else if (obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) > 149 && obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) <= 150) {
          desenhaCirculoMenor(118, 249, 251);
        } else if(obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) < 150) {
          desenhaCirculoMenor(255, 255, 0);
        }
        else {
          this.ptoCentral.Y -= 1;
        }
      }
      else if (e.Key == Key.D) {
        this.ptoCentral.X += 1;
        if (obj_BBox.validaDentro(this.ptoCentral)) {
          desenhaCirculoMenor(255, 0, 255);
        } else if (obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) > 149 && obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) <= 150) {
          desenhaCirculoMenor(118, 249, 251);
        } else if(obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) < 150) {
          desenhaCirculoMenor(255, 255, 0);
        }
        else {
          this.ptoCentral.X -= 1;
        }
      }
      else if (e.Key == Key.S) {
        this.ptoCentral.Y -= 1;
        if (obj_BBox.validaDentro(this.ptoCentral)) {
          desenhaCirculoMenor(255, 0, 255);
        } else if (obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) > 149 && obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) <= 150) {
          desenhaCirculoMenor(118, 249, 251);
        } else if(obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) < 150) {
          desenhaCirculoMenor(255, 255, 0);
        }
        else {
          this.ptoCentral.Y += 1;
        }
      }
#if CG_Gizmo
      else if (e.Key == Key.O)
        bBoxDesenhar = !bBoxDesenhar;
#endif
      else if (e.Key == Key.V)
        mouseMoverPto = !mouseMoverPto;   //TODO: falta atualizar a BBox do objeto
      else
        Console.WriteLine(" __ Tecla não implementada.");
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
      Ponto4D move_mouse = new Ponto4D(mouseX, mouseY);
      /*if (obj_BBox.validaDentro(move_mouse)) {
          objetosLista.Remove(obj_Circulo_Menor);
          objetoId = Utilitario.charProximo(objetoId);
          obj_Circulo_Menor = new Ciurculo(objetoId, null, ptoCentral, 50, 36, true);
          obj_Circulo_Menor.ObjetoCor.CorR = 0; obj_Circulo_Menor.ObjetoCor.CorG = 0; obj_Circulo_Menor.ObjetoCor.CorB =0;
          obj_Circulo_Menor.PrimitivaTipo = PrimitiveType.LineStrip;
          obj_Circulo_Menor.PrimitivaTamanho = 5;
          objetosLista.Add(obj_Circulo_Menor);
          obj_Retangulo.ObjetoCor.CorR = 255;obj_Retangulo.ObjetoCor.CorG = 0;obj_Retangulo.ObjetoCor.CorB = 255;
        } else if (obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) == 150) {
                    Console.WriteLine("Raio");
          obj_Retangulo.ObjetoCor.CorR = 0;obj_Retangulo.ObjetoCor.CorG = 0;obj_Retangulo.ObjetoCor.CorB = 0;
        } else if (obj_Circulo_Maior.distanciaEuclediana(this.ptoCentral) < 150){
                    Console.WriteLine("entre bbob e raio");
          objetosLista.Remove(obj_Circulo_Menor);
          objetoId = Utilitario.charProximo(objetoId);
          obj_Circulo_Menor = new Ciurculo(objetoId, null, ptoCentral, 50, 36, true);
          obj_Circulo_Menor.ObjetoCor.CorR = 0; obj_Circulo_Menor.ObjetoCor.CorG = 0; obj_Circulo_Menor.ObjetoCor.CorB =0;
          obj_Circulo_Menor.PrimitivaTipo = PrimitiveType.LineStrip;
          obj_Circulo_Menor.PrimitivaTamanho = 5;
          objetosLista.Add(obj_Circulo_Menor);
          obj_Retangulo.ObjetoCor.CorR = 255;obj_Retangulo.ObjetoCor.CorG = 255;obj_Retangulo.ObjetoCor.CorB = 0;
        } else {
          Console.WriteLine("Não entrei lá!");
        }*/
      if (mouseMoverPto && (objetoSelecionado != null))
      {
        objetoSelecionado.PontosUltimo().X = mouseX;
        objetoSelecionado.PontosUltimo().Y = mouseY;
      }
    }

    private void desenhaCirculoMenor(byte corR, byte corG, byte corB ) {
      objetosLista.Remove(obj_Circulo_Menor);
      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo_Menor = new Ciurculo(objetoId, null, ptoCentral, 50, 36, true);
      obj_Circulo_Menor.ObjetoCor.CorR = 0; obj_Circulo_Menor.ObjetoCor.CorG = 0; obj_Circulo_Menor.ObjetoCor.CorB =0;
      obj_Circulo_Menor.PrimitivaTipo = PrimitiveType.LineStrip;
      obj_Circulo_Menor.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo_Menor);
      obj_Retangulo.ObjetoCor.CorR = corR;obj_Retangulo.ObjetoCor.CorG = corG;obj_Retangulo.ObjetoCor.CorB = corB;
    }

#if CG_Gizmo
    private void Sru3D()
    {
#if CG_OpenGL
      GL.LineWidth(1);
      GL.Begin(PrimitiveType.Lines);
      // GL.Color3(1.0f,0.0f,0.0f);
      GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      // GL.Color3(0.0f,1.0f,0.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      // GL.Color3(0.0f,0.0f,1.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();
#endif
    }
#endif    
  }
  class Program
  {
    static void Main(string[] args)
    {
      ToolkitOptions.Default.EnableHighResolution = false;
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N2";
      window.Run(1.0 / 60.0);
    }
  }
}
