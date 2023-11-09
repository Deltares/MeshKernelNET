﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public class MakeGridParameters : INotifyPropertyChanged
    {
        private int numberOfRows;
        private int numberOfColumns;
        private double gridAngle;
        private double originXCoordinate;
        private double originYCoordinate;
        private double xGridBlockSize;
        private double yGridBlockSize;
        private double upperRightCornerXCoordinate;
        private double upperRightCornerYCoordinate;

        private GridTypeOptions gridType;

        public static MakeGridParameters CreateDefault()
        {
            return new MakeGridParameters
            {
                GridType = GridTypeOptions.Square,
                NumberOfColumns = 3,
                NumberOfRows = 3,
                GridAngle = 0.0,
                OriginXCoordinate = 0.0,
                OriginYCoordinate = 0.0,
                XGridBlockSize = 10.0,
                YGridBlockSize = 10.0,
                UpperRightCornerXCoordinate = 0.0,
                UpperRightCornerYCoordinate = 0.0
            };
        }

        /// <summary>
        /// * The type of grid to create : square = 0, wieber = 1, hexagonal type 1 = 2,  hexagonal type 2 = 3, triangular = 4 (0)
        /// </summary>
        [ProtoMember(1)]
        public GridTypeOptions GridType
        {
            get { return gridType; }
            set
            {
                gridType = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The number of columns in x direction (0)
        /// </summary>
        [ProtoMember(2)]
        public int NumberOfColumns
        {
            get { return numberOfColumns; }
            set
            {
                numberOfColumns = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The number of columns in y direction (0)
        /// </summary>
        [ProtoMember(3)]
        public int NumberOfRows
        {
            get { return numberOfRows; }
            set
            {
                numberOfRows = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The grid angle (0.0)
        /// </summary>
        [ProtoMember(4)]
        public double GridAngle
        {
            get { return gridAngle; }
            set
            {
                gridAngle = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The x coordinate of the origin, located at the bottom left corner (0.0)
        /// </summary>
        [ProtoMember(5)]
        public double OriginXCoordinate
        {
            get { return originXCoordinate; }
            set
            {
                originXCoordinate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The y coordinate of the origin, located at the bottom left corner (0.0)
        /// </summary>
        [ProtoMember(6)]
        public double OriginYCoordinate
        {
            get { return originYCoordinate; }
            set
            {
                originYCoordinate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The grid block size in x dimension, used only for squared grids (0.0)
        /// </summary>
        [ProtoMember(7)]
        public double XGridBlockSize
        {
            get { return xGridBlockSize; }
            set
            {
                xGridBlockSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// * The grid block size in y dimension, used only for squared grids (0.0)
        /// </summary>
        [ProtoMember(8)]
        public double YGridBlockSize
        {
            get { return yGridBlockSize; }
            set
            {
                yGridBlockSize = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// *  The x coordinate of the upper right corner (0.0)
        /// </summary>
        [ProtoMember(9)]
        public double UpperRightCornerXCoordinate
        {
            get { return upperRightCornerXCoordinate; }
            set
            {
                upperRightCornerXCoordinate = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// *  The x coordinate of the upper right corner (0.0)
        /// </summary>
        [ProtoMember(10)]
        public double UpperRightCornerYCoordinate
        {
            get { return upperRightCornerYCoordinate; }
            set
            {
                upperRightCornerYCoordinate = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}