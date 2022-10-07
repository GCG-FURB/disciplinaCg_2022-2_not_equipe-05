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
    private SegReta obj_segReta_esquerda;
    private SegReta obj_segReta_Cima;
    private SegReta obj_segReta_Direita;
    private Ponto4D pto_esquerdaBaixo = new Ponto4D(-100, -100, 0);
    private Ponto4D pto_esquerdaCima = new Ponto4D(-100, 100, 0);
    private Ponto4D pto_direitaCima = new Ponto4D(100, 100, 0);
    private Ponto4D pto_direitaBaixo = new Ponto4D(100, -100, 0);
    private PontoGeometrico obj_PontoEsquerdaBaixo;
    private PontoGeometrico obj_PontoEsquerdaCima;
    private PontoGeometrico obj_PontoDireitaCima;
    private PontoGeometrico obj_PontoDireitaBaixo;
    private Ponto4D pontoSelecionado;
    private Spline obj_spline;
    private int qntPontosSpline = 1;
    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -400; camera.xmax = 400; camera.ymin = -400; camera.ymax = 400;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      this.desenharObjeto();

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

    private void desenharObjeto() {
      objetoId = Utilitario.charProximo(objetoId);
      obj_segReta_esquerda = new SegReta(objetoId, null, pto_esquerdaBaixo, pto_esquerdaCima);
      obj_segReta_esquerda.ObjetoCor.CorR = 118; obj_segReta_esquerda.ObjetoCor.CorG = 249; obj_segReta_esquerda.ObjetoCor.CorB  = 251;
      obj_segReta_esquerda.PrimitivaTipo = PrimitiveType.Lines;
      obj_segReta_esquerda.PrimitivaTamanho = 5;
      objetosLista.Add(obj_segReta_esquerda);
      objetoSelecionado = obj_segReta_esquerda;

      objetoId = Utilitario.charProximo(objetoId);
      obj_segReta_Cima = new SegReta(objetoId, null, pto_esquerdaCima, pto_direitaCima);
      obj_segReta_Cima.ObjetoCor.CorR = 118; obj_segReta_Cima.ObjetoCor.CorG = 249; obj_segReta_Cima.ObjetoCor.CorB  = 251;
      obj_segReta_Cima.PrimitivaTipo = PrimitiveType.Lines;
      obj_segReta_Cima.PrimitivaTamanho = 5;
      objetosLista.Add(obj_segReta_Cima);
      objetoSelecionado = obj_segReta_Cima;

      objetoId = Utilitario.charProximo(objetoId);
      obj_segReta_Direita = new SegReta(objetoId, null, pto_direitaBaixo, pto_direitaCima);
      obj_segReta_Direita.ObjetoCor.CorR = 118; obj_segReta_Direita.ObjetoCor.CorG = 249; obj_segReta_Direita.ObjetoCor.CorB  = 251;
      obj_segReta_Direita.PrimitivaTipo = PrimitiveType.Lines;
      obj_segReta_Direita.PrimitivaTamanho = 5;
      objetosLista.Add(obj_segReta_Direita);
      objetoSelecionado = obj_segReta_Direita;

      objetoId = Utilitario.charProximo(objetoId);
      obj_PontoEsquerdaBaixo = new PontoGeometrico(objetoId, null, pto_esquerdaBaixo);
      obj_PontoEsquerdaBaixo.ObjetoCor.CorR = 0;obj_PontoEsquerdaBaixo.ObjetoCor.CorG = 0; obj_PontoEsquerdaBaixo.ObjetoCor.CorB = 0;
      obj_PontoEsquerdaBaixo.PrimitivaTamanho = 7;
      obj_PontoEsquerdaBaixo.PrimitivaTipo = PrimitiveType.Points;
      objetosLista.Add(obj_PontoEsquerdaBaixo);
      objetoSelecionado = obj_PontoEsquerdaBaixo;

      objetoId = Utilitario.charProximo(objetoId);
      obj_PontoEsquerdaCima = new PontoGeometrico(objetoId, null, pto_esquerdaCima);
      obj_PontoEsquerdaCima.ObjetoCor.CorR = 0;obj_PontoEsquerdaCima.ObjetoCor.CorG = 0; obj_PontoEsquerdaCima.ObjetoCor.CorB = 0;
      obj_PontoEsquerdaCima.PrimitivaTamanho = 7;
      obj_PontoEsquerdaCima.PrimitivaTipo = PrimitiveType.Points;
      objetosLista.Add(obj_PontoEsquerdaCima);
      objetoSelecionado = obj_PontoEsquerdaCima;

      objetoId = Utilitario.charProximo(objetoId);
      obj_PontoDireitaCima = new PontoGeometrico(objetoId, null, pto_direitaCima);
      obj_PontoDireitaCima.ObjetoCor.CorR = 0;obj_PontoDireitaCima.ObjetoCor.CorG = 0; obj_PontoDireitaCima.ObjetoCor.CorB = 0;
      obj_PontoDireitaCima.PrimitivaTamanho = 7;
      obj_PontoDireitaCima.PrimitivaTipo = PrimitiveType.Points;
      objetosLista.Add(obj_PontoDireitaCima);
      objetoSelecionado = obj_PontoDireitaCima;

      objetoId = Utilitario.charProximo(objetoId);
      obj_PontoDireitaBaixo = new PontoGeometrico(objetoId, null, pto_direitaBaixo);
      obj_PontoDireitaBaixo.ObjetoCor.CorR = 0;obj_PontoDireitaBaixo.ObjetoCor.CorG = 0; obj_PontoDireitaBaixo.ObjetoCor.CorB = 0;
      obj_PontoDireitaBaixo.PrimitivaTamanho = 5;
      obj_PontoDireitaBaixo.PrimitivaTipo = PrimitiveType.Points;
      objetosLista.Add(obj_PontoDireitaBaixo);
      objetoSelecionado = obj_PontoDireitaBaixo;

      objetoId = Utilitario.charProximo(objetoId);
      obj_spline = new Spline(objetoId, null, pto_esquerdaBaixo, pto_esquerdaCima, pto_direitaCima, pto_direitaBaixo, qntPontosSpline);
      obj_spline.ObjetoCor.CorR = 255;obj_spline.ObjetoCor.CorG = 255; obj_spline.ObjetoCor.CorB = 0;
      obj_spline.PrimitivaTamanho = 7;
      obj_spline.PrimitivaTipo = PrimitiveType.LineStrip;
      objetosLista.Add(obj_spline);
      objetoSelecionado = obj_spline;

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
        pontoSelecionado.X -=1;
        for (var i = 0; i < objetosLista.Count; i++)
        {
          Console.WriteLine(objetosLista[i]);
        }
      }
      else if (e.Key == Key.Number1) {
        this.pintarPontosPreto();
        obj_PontoEsquerdaBaixo.ObjetoCor.CorR = 255;
        pontoSelecionado = pto_esquerdaBaixo;
      }
      else if (e.Key == Key.Number2) {
        this.pintarPontosPreto();
        obj_PontoEsquerdaCima.ObjetoCor.CorR = 255;
        pontoSelecionado = pto_esquerdaCima;
      }
      else if (e.Key == Key.Number3) {
        this.pintarPontosPreto();
        obj_PontoDireitaCima.ObjetoCor.CorR = 255;
        pontoSelecionado = pto_direitaCima;
      }
      else if (e.Key == Key.Number4) {
        this.pintarPontosPreto();
        obj_PontoDireitaBaixo.ObjetoCor.CorR = 255;
        pontoSelecionado = pto_direitaBaixo;
      }
      else if (e.Key == Key.C) {
        pontoSelecionado.Y += 1;
      }
      else if (e.Key == Key.B) {
        pontoSelecionado.Y -=1;
      }
      else if (e.Key == Key.E) {
      }
      else if (e.Key == Key.D) {
        pontoSelecionado.X += 1;
      }
      else if (e.Key == Key.U) {
        qntPontosSpline+= 1;
        criaSpline();
      }
      else if (e.Key == Key.I) {
        qntPontosSpline -= 1;
        criaSpline();
        Console.WriteLine(qntPontosSpline);
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

    private void criaSpline() {
      objetosLista.Remove(obj_spline);
      objetoId = Utilitario.charProximo(objetoId);
      obj_spline = new Spline(objetoId, null, pto_esquerdaBaixo, pto_esquerdaCima, pto_direitaCima, pto_direitaBaixo, qntPontosSpline);
      obj_spline.ObjetoCor.CorR = 255;obj_spline.ObjetoCor.CorG = 255; obj_spline.ObjetoCor.CorB = 0;
      obj_spline.PrimitivaTamanho = 7;
      obj_spline.PrimitivaTipo = PrimitiveType.LineStrip;
      objetosLista.Add(obj_spline);
      objetoSelecionado = obj_spline;
    }
    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
      if (mouseMoverPto && (objetoSelecionado != null))
      {
        objetoSelecionado.PontosUltimo().X = mouseX;
        objetoSelecionado.PontosUltimo().Y = mouseY;
      }
    }

    private void pintarPontosPreto() {
      obj_PontoDireitaBaixo.ObjetoCor.CorR = 0;obj_PontoDireitaBaixo.ObjetoCor.CorG = 0;obj_PontoDireitaBaixo.ObjetoCor.CorB = 0;
      obj_PontoDireitaCima.ObjetoCor.CorR = 0;obj_PontoDireitaCima.ObjetoCor.CorG = 0;obj_PontoDireitaCima.ObjetoCor.CorB = 0;
      obj_PontoEsquerdaBaixo.ObjetoCor.CorR = 0;obj_PontoEsquerdaBaixo.ObjetoCor.CorG = 0;obj_PontoEsquerdaBaixo.ObjetoCor.CorB = 0;
      obj_PontoEsquerdaCima.ObjetoCor.CorR = 0;obj_PontoEsquerdaCima.ObjetoCor.CorG = 0;obj_PontoEsquerdaCima.ObjetoCor.CorB = 0;
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
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N2";
      window.Run(1.0 / 60.0);
    }
  }
}
