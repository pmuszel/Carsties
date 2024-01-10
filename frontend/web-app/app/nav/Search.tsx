'use client'

import { useParamsStore } from '@/hooks/useParamsStore'
import React, { useState } from 'react'
import {FaSearch} from 'react-icons/fa'

export default function Search() {
    const setParams = useParamsStore(states => states.setParams);
    const searchValue = useParamsStore(states => states.searchValue);
    const setSearchValue = useParamsStore(states => states.setSearchValue);

    //const [value, setValue] = useState('');

    function onChange(event: any) {
        setSearchValue(event.target.value);
    }

    function search() {
        setParams({searchTerm: searchValue});
    }

  return (
    <div className='flex w-[50%] items-center border-2 rounded-full py-2 shadow-sm'>
        <input 
            type='text' 
            placeholder='Search for cars by make, model od color'
            className='
                flex-grow
                pl-5
                bg-transparent
                focus:outline-none
                border-transparent
                focus:border-transparent
                focus:ring-0
                text-sm
                text-gray-600
            '
            value={searchValue}
            onChange={onChange}
            onKeyDown={(e: any) => {
                console.log(e.key);
                if(e.key === 'Enter') search()
            }}
        />
        <button onClick={search}>
            <FaSearch size={34} 
                className='bg-red-400 text-white rounded-full cursor-pointer mx-2 p-2 hover:bg-red-700' />
        </button>
    </div>
  )
}
