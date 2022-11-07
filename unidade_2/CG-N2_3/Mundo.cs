﻿/**
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
    private Ciurculo obj_Circulo;
    private SegReta obj_SegReta;
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -300; camera.xmax = 300; camera.ymin = -300; camera.ymax = 300;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);
      obj_Retangulo = new Retangulo(objetoId, null, new Ponto4D(50, 50, 0), new Ponto4D(150, 150, 0));
      obj_Retangulo.ObjetoCor.CorR = 255; obj_Retangulo.ObjetoCor.CorG = 0; obj_Retangulo.ObjetoCor.CorB = 255;
      objetosLista.Add(obj_Retangulo);
    

      objetoId = Utilitario.charProximo(objetoId);
      Ponto4D ptoCentroCirculo1 = new Ponto4D(0, 100, 0);
      obj_Circulo = new Ciurculo(objetoId, null, ptoCentroCirculo1 , 100);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 0; obj_Circulo.ObjetoCor.CorB = 0;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;


      Ponto4D ptoCentroCirculo2 = new Ponto4D(100, -100, 0);
      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Ciurculo(objetoId, null, ptoCentroCirculo2, 100);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 0; obj_Circulo.ObjetoCor.CorB = 0;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;

      objetoId = Utilitario.charProximo(objetoId);
      Ponto4D ptoCentroCirculo3 = new Ponto4D(-100, -100, 0);
      obj_Circulo = new Ciurculo(objetoId, null, ptoCentroCirculo3, 100);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 0; obj_Circulo.ObjetoCor.CorB = 0;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
  
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new SegReta(objetoId, null, ptoCentroCirculo1, ptoCentroCirculo2);
      obj_SegReta.ObjetoCor.CorR = 118; obj_SegReta.ObjetoCor.CorG = 249; obj_SegReta.ObjetoCor.CorB  = 251;
      obj_SegReta.PrimitivaTipo = PrimitiveType.Lines;
      obj_SegReta.PrimitivaTamanho = 5;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;
 
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new SegReta(objetoId, null, ptoCentroCirculo2, ptoCentroCirculo3);
      obj_SegReta.ObjetoCor.CorR = 118; obj_SegReta.ObjetoCor.CorG = 249; obj_SegReta.ObjetoCor.CorB  = 251;
      obj_SegReta.PrimitivaTipo = PrimitiveType.Lines;
      obj_SegReta.PrimitivaTamanho = 5;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new SegReta(objetoId, null, ptoCentroCirculo3, ptoCentroCirculo1);
      obj_SegReta.ObjetoCor.CorR = 118; obj_SegReta.ObjetoCor.CorG = 249; obj_SegReta.ObjetoCor.CorB  = 251;
      obj_SegReta.PrimitivaTipo = PrimitiveType.Lines;
      obj_SegReta.PrimitivaTamanho = 5;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
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
        if (camera.xmax < Double.MaxValue && camera.xmin < Double.MaxValue) {
          camera.PanEsquerda();
        }
      }
      else if (e.Key == Key.D) {
        if (camera.xmax > Double.MinValue && camera.xmin > Double.MinValue) {
          camera.PanDireita();
        }
      }
      else if (e.Key == Key.C) {
        if (camera.ymin > Double.MinValue && camera.ymax > Double.MinValue) {
          camera.PanCima();
        }
      }
      else if (e.Key == Key.B) {
        if (camera.ymin < Double.MaxValue && camera.ymax < Double.MaxValue) {
          camera.PanBaixo();
        }
      }
      else if (e.Key == Key.I) {
        if (camera.xmin < Double.MaxValue && camera.xmax > Double.MinValue && camera.ymin < Double.MaxValue && camera.ymax > Double.MinValue) {
          camera.ZoomIn();
        }
      }
      else if (e.Key == Key.O) {
        if (camera.xmin > Double.MinValue && camera.xmax < Double.MaxValue && camera.ymin > Double.MinValue && camera.ymax < Double.MaxValue) {
          camera.ZoomOut();
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
      if (mouseMoverPto && (objetoSelecionado != null))
      {
        objetoSelecionado.PontosUltimo().X = mouseX;
        objetoSelecionado.PontosUltimo().Y = mouseY;
      }
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
