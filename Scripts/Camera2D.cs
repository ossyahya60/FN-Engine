using Microsoft.Xna.Framework;
using System.IO;

namespace FN_Engine
{
    public class Camera2D
    {
        private float _zoom;
        private float _rotation;
        private Vector2 _position;
        private Matrix _transform = Matrix.Identity;
        private bool _isViewTransformationDirty = true;
        private Matrix _camTranslationMatrix = Matrix.Identity;
        private Matrix _camRotationMatrix = Matrix.Identity;
        private Matrix _camScaleMatrix = Matrix.Identity;
        private Matrix _resTranslationMatrix = Matrix.Identity;
        private Vector3 _camTranslationVector = Vector3.Zero;
        private Vector3 _camScaleVector = Vector3.Zero;
        private Vector3 _resTranslationVector = Vector3.Zero;

        public Camera2D()
        {
            _zoom = 0.1f;
            _rotation = 0.0f;
            _position = Vector2.Zero;
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                    _isViewTransformationDirty = true;
                _position = value;
            }
        }

        public void Move(Vector2 amount, float GameTime)
        {
            Position += amount * GameTime;
        }

        public void SetPosition(Vector2 position)
        {
            Position = position;
        }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                if(_zoom != value)
                    _isViewTransformationDirty = true;

                _zoom = value;
                if (_zoom < 0.1f)
                {
                    _zoom = 0.1f;
                }
            }
        }

        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                if(_rotation != value)
                    _isViewTransformationDirty = true;

                _rotation = value;
            }
        }

        public Matrix GetViewTransformationMatrix()
        {
            if (_isViewTransformationDirty)
            {
                _camTranslationVector.X = -_position.X;
                _camTranslationVector.Y = -_position.Y;

                Matrix.CreateTranslation(ref _camTranslationVector, out _camTranslationMatrix);
                Matrix.CreateRotationZ(_rotation, out _camRotationMatrix);

                _camScaleVector.X = _zoom;
                _camScaleVector.Y = _zoom;
                _camScaleVector.Z = 1;

                Matrix.CreateScale(ref _camScaleVector, out _camScaleMatrix);

                _resTranslationVector.X = ResolutionIndependentRenderer.GetVirtualRes().X * 0.5f;
                _resTranslationVector.Y = ResolutionIndependentRenderer.GetVirtualRes().Y * 0.5f;
                _resTranslationVector.Z = 0;

                Matrix.CreateTranslation(ref _resTranslationVector, out _resTranslationMatrix);

                _transform = _camTranslationMatrix *
                             _camRotationMatrix *
                             _camScaleMatrix *
                             _resTranslationMatrix *
                             ResolutionIndependentRenderer.getTransformationMatrix();

                _isViewTransformationDirty = false;
            }
            
            return _transform;
        }

        public void RecalculateTransformationMatrices()
        {
            _isViewTransformationDirty = true;
        }

        public void Serialize(StreamWriter SW)
        {
            SW.WriteLine(ToString());

            SW.Write("_zoom:\t" + _zoom.ToString() + "\n");
            SW.Write("_rotation:\t" + _rotation.ToString() + "\n");
            SW.Write("_position:\t" + _position.X.ToString() + "\t" + _position.Y.ToString() + "\n");

            SW.WriteLine("End Of " + ToString());
        }
    }
}