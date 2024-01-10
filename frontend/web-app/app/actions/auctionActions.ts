'use server'

import { Auction, PageResult } from "@/types";

export async function getData(query: string): Promise<PageResult<Auction>> {
    const url = `http://localhost:6001/search${query}`;

    console.log(url);

    const res = await fetch(url);

    if(!res.ok) throw new Error('Failed to fetch data');

    return res.json();
}