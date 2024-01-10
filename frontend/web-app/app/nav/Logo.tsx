'use client'

import { useParamsStore } from '@/hooks/useParamsStore';
import React from 'react'
import { IoCarSportSharp } from 'react-icons/io5'

export default function Logo() {
    const reset = useParamsStore(state => state.reset);
    
  return (
        <div className='flex items-center gap-2 text-3xl font-semibold text-red-500 cursor-pointer' onClick={reset}>
                <IoCarSportSharp size={34}/>
                <div>Carsties Auctions</div>
        </div>
  )
}
