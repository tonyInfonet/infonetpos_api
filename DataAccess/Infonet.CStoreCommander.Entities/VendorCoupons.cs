using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Infonet.CStoreCommander.Entities
{
    public class VendorCoupons : IEnumerable<VendorCoupon> 
    {

        private Collection mCol;
      
        public VendorCoupon this[object vntIndexKey]
        {
            get
            {
                VendorCoupon returnValue = default(VendorCoupon);
                returnValue = mCol[vntIndexKey] as VendorCoupon;
                return returnValue;
            }
        }

        public int Count
        {
            get
            {
                int returnValue = 0;
                returnValue = mCol.Count;
                return returnValue;
            }
        }

        //Public ReadOnly Property NewEnum() As stdole.IUnknown
        //Get
        // ExcludeSE
        //NewEnum = mCol._NewEnum
        //End Get
        //End Property

        public System.Collections.IEnumerator GetEnumerator()
        {
           return mCol.GetEnumerator();
            //_index = 0;
            //return this;
        }

        public VendorCoupon AddCoupon(short IdNum, VendorCoupon oCoupon, string sKey)
        {
            VendorCoupon returnValue = default(VendorCoupon);

            oCoupon.IdNum = IdNum;

            if (sKey.Length == 0)
            {
                mCol.Add(oCoupon, null, null, null);
            }
            else
            {
                mCol.Add(oCoupon, sKey, null, null);
            }

            //return the object created
            returnValue = oCoupon;

            return returnValue;
        }

        public void Remove(object vntIndexKey)
        {
            mCol.Remove(System.Convert.ToString(vntIndexKey));
        }

        private void Class_Initialize_Renamed()
        {
            mCol = new Collection();
        }
        public VendorCoupons()
        {
            Class_Initialize_Renamed();
        }

        private void Class_Terminate_Renamed()
        {
            mCol = null;
        }

        public void Dispose()
        {
           
            //base.Finalize();
        }
        ~VendorCoupons()
        {
            Class_Terminate_Renamed();
        }

       // public void Load()
       // {
            //ADODB.Recordset rsCoupon = default(ADODB.Recordset);
            //VendorCoupon VC = default(VendorCoupon);

            //rsCoupon = Chaps_Main.Get_Records("select * from VendorCoupons", Chaps_Main.dbMaster, (System.Int32)ADODB.CursorTypeEnum.adOpenForwardOnly, (System.Int32)ADODB.LockTypeEnum.adLockReadOnly);

            //while (!rsCoupon.EOF)
            //{
            //    VC = new VendorCoupon();
            //    VC.Code = System.Convert.ToString(rsCoupon.Fields["Code"].Value);
            //    VC.Name = System.Convert.ToString((Information.IsDBNull(rsCoupon.Fields["Name"].Value)) ? "" : (rsCoupon.Fields["Name"].Value));
            //    VC.VendorCode = System.Convert.ToString((Information.IsDBNull(rsCoupon.Fields["VendorCode"].Value)) ? "" : (rsCoupon.Fields["VendorCode"].Value));
            //    VC.Value = System.Convert.ToSingle(rsCoupon.Fields["Value"].Value);
            //    VC.StockCode = System.Convert.ToString(rsCoupon.Fields["StockCode"].Value);
            //    VC.Dept = System.Convert.ToString(rsCoupon.Fields["Dept"].Value);
            //    VC.SubDept = System.Convert.ToString(rsCoupon.Fields["SubDept"].Value);
            //    VC.SubDetail = System.Convert.ToString(rsCoupon.Fields["SubDetail"].Value);
            //    VC.TendDesc = System.Convert.ToString((Information.IsDBNull(rsCoupon.Fields["TendDesc"].Value)) ? "" : (rsCoupon.Fields["TendDesc"].Value)); 
            //                                                                                                                                                 
            //    VC.DefaultCoupon = System.Convert.ToBoolean((Information.IsDBNull(rsCoupon.Fields["DefaultCoupon"].Value)) ? false : (rsCoupon.Fields["DefaultCoupon"].Value));
            //    VC.SerNumLen = System.Convert.ToInt16((Information.IsDBNull(rsCoupon.Fields["SerialNumLen"].Value)) ? 0 : (rsCoupon.Fields["SerialNumLen"].Value));
            //    
              // this.AddCoupon((short)(this.Count + 1), VC, "");

            //    rsCoupon.MoveNext();
            //}

            //rsCoupon = null;
            //VC = null;
       // }

      

        IEnumerator<VendorCoupon> IEnumerable<VendorCoupon>.GetEnumerator()
        {
            return new VendorCouponEnumerator(mCol);
        }
    }


    public class VendorCouponEnumerator : IEnumerator<VendorCoupon>, IDisposable
    {
        private Collection mCol;
        private int _index;
        private VendorCoupon _current;

        public VendorCouponEnumerator(Collection col)
        {
            mCol = col;
        }

        public VendorCoupon Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return _current;
            }
        }

        public void Dispose()
        {
            mCol = null;
        }

        public bool MoveNext()
        {
            _index++;
            if (_index <= mCol.Count)
            {
                _current = mCol[_index] as VendorCoupon;
            }
            else
            {
                _current = null;
            }
            return (_index <= mCol.Count);
        }

        public void Reset()
        {
            _current = null;
            _index = 0;
        }
    }
}
